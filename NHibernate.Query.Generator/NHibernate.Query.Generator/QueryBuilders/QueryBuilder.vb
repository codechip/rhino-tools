Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Reflection
Imports NHibernate
Imports NHibernate.Expression
Imports NHibernate.Type

Namespace Query

#Region "QueryBuilder<T>"

	Public Class QueryBuilder(Of T)
#Region "Fields"

		Protected Friend where As WhereClause(Of T)
		Protected Friend projection As ProjectionClauseProperty(Of T)
		Protected Friend order As OrderByClauseProperty(Of T)
		Protected Friend subqueries As ICollection(Of ICriterion) = New List(Of ICriterion)()
		Protected Friend whereHasValue As Boolean
		Protected Friend projectionHasValue As Boolean
		Protected Friend orderHasValue As Boolean

#End Region

#Region "Builder Methods"

#Region "Append Where Methods"

		Public Overridable Function Where(ByVal whereClause As WhereClause(Of T)) As QueryBuilder(Of T)
			If where Then
				where = where AndAlso whereClause
			Else
				where = whereClause
			End If
			whereHasValue = True
			Return Me
		End Function

		Public Overridable Function [And](ByVal whereClause As WhereClause(Of T)) As QueryBuilder(Of T)
			Return Where(whereClause)
		End Function

		Public Overridable Function [Or](ByVal whereClause As WhereClause(Of T)) As QueryBuilder(Of T)
			If Not whereHasValue Then
				Return Where(whereClause)
			End If
			where = where OrElse whereClause
			Return Me
		End Function

#End Region

#Region "Append Projection Methods"

		Public Overridable Function ProjectBy(ByVal projectByClause As ProjectionClauseProperty(Of T)) As QueryBuilder(Of T)
			If Not projectionHasValue Then
				projection = projectByClause
			Else
				projection = projection AndAlso projectByClause
			End If
			projectionHasValue = True
			Return Me
		End Function



#End Region

#Region "Append OrderBy Methods"

		Public Overridable Function Order(ByVal orderByClause As OrderByClauseProperty(Of T)) As QueryBuilder(Of T)
			If Not orderHasValue Then
				order = orderByClause
			Else
				order = order AndAlso orderByClause
			End If
			orderHasValue = True
			Return Me
		End Function

		Public Overridable Function NotExists(ByVal subQuery As Subquery) As QueryBuilder(Of T)
			subqueries.Add(Subqueries.NotExists(subQuery))
			Return Me
		End Function

		Public Overridable Function Exists(ByVal subQuery As Subquery) As QueryBuilder(Of T)
			subqueries.Add(Subqueries.Exists(subQuery))
			Return Me
		End Function

		Public Function WithResultType(Of RT)() As CustomResultQueryBuilder(Of T, RT)
			Return New CustomResultQueryBuilder(Of T, RT)(Me)
		End Function

		Public Function WithResultType(Of RT)(ByVal ci As ConstructorInfo) As CustomResultQueryBuilder(Of T, RT)
			Return New CustomResultQueryBuilder(Of T, RT)(Me, ci)
		End Function

#End Region

#End Region

#Region "Reset Query Builder"

		Public Overridable Sub ResetSubqueries()
			subqueries.Clear()
		End Sub

		Public Overridable Sub ResetOrderBy()
			order = Nothing
			orderHasValue = False
		End Sub

		Public Overridable Sub ResetProjections()
			projection = Nothing
			projectionHasValue = False
		End Sub

		Public Overridable Sub ResetWhere()
			where = Nothing
			whereHasValue = False
		End Sub

		Public Overridable Sub Reset()
			ResetWhere()
			ResetProjections()
			ResetOrderBy()
			ResetSubqueries()
		End Sub

#End Region

#Region "Casting Magic"

		Public Shared Widening Operator CType(ByVal expr As QueryBuilder(Of T)) As DetachedCriteria
			Return expr.ToDetachedCriteria(Nothing)
		End Operator

		Public Shared Widening Operator CType(ByVal expr As QueryBuilder(Of T)) As Subquery
			Return New Subquery(expr.ToDetachedCriteria(Nothing))
		End Operator

		Public Overridable Function ToDetachedCriteria() As DetachedCriteria
			Return ToDetachedCriteria(Nothing)
		End Function

		Public Overridable Function ToDetachedCriteria(ByVal [alias] As String) As DetachedCriteria
			Dim detachedCriteria As DetachedCriteria
			If [String].IsNullOrEmpty([alias]) Then
				detachedCriteria = DetachedCriteria.[For](GetType(T))
			Else
				detachedCriteria = DetachedCriteria.[For](GetType(T), [alias])
			End If

			Dim criterias As IDictionary(Of String, DetachedCriteria) = New Dictionary(Of String, DetachedCriteria)()

			If whereHasValue Then
				where.BuildQuery(detachedCriteria, criterias)
			End If

			If projectionHasValue Then
				projection.BuildQuery(detachedCriteria, criterias)
			End If

			If orderHasValue Then
				order.BuildQuery(detachedCriteria, criterias)
			End If

			For Each c As ICriterion In subqueries
				detachedCriteria.Add(c)
			Next

			Return detachedCriteria
		End Function

#End Region
	End Class

	Public Class CustomResultQueryBuilder(Of T, RT)
		Inherits QueryBuilder(Of T)
		Private ci As ConstructorInfo

		Protected Friend Sub New(ByVal inner As QueryBuilder(Of T))
			Me.New(inner, GetType(RT).GetConstructors()(0))
		End Sub

		Protected Friend Sub New(ByVal inner As QueryBuilder(Of T), ByVal ci As ConstructorInfo)
			Me.order = inner.order
			Me.projection = inner.projection
			Me.where = inner.where
			Me.orderHasValue = inner.orderHasValue
			Me.projectionHasValue = inner.projectionHasValue
			Me.whereHasValue = inner.whereHasValue
			Me.subqueries = inner.subqueries
			Me.ci = ci
		End Sub

		Public Overloads Overrides Function ToDetachedCriteria(ByVal [alias] As String) As DetachedCriteria
			Return MyBase.ToDetachedCriteria([alias]).SetResultTransformer(New NHibernate.Transform.AliasToBeanConstructorResultTransformer(ci))
		End Function

		Public Function List(ByVal session As ISession) As IList(Of RT)
			Return ToDetachedCriteria().GetExecutableCriteria(session).List(Of RT)()
		End Function

		Public Function UniqueResult(ByVal session As ISession) As RT
			Return ToDetachedCriteria().GetExecutableCriteria(session).UniqueResult(Of RT)()
		End Function
	End Class

#End Region

#Region "QueryPart"

	Public MustInherit Class QueryPart
#Region "Fields"

		Protected Friend name As String
		Protected Friend backTrack As Boolean
		Protected Friend associationPath As String

#End Region

#Region "ctor"

		Public Sub New(ByVal name As String, ByVal associationPath As String)
			Me.New(name, associationPath, False)
		End Sub

		Public Sub New(ByVal name As String, ByVal associationPath As String, ByVal backTrack As Boolean)
			Me.name = name
			Me.associationPath = IIf(associationPath Is Nothing, "this", associationPath)
			Me.backTrack = backTrack
		End Sub

#End Region

#Region "Properties"

		Protected Friend Overridable ReadOnly Property Name() As String
			Get
				Return name
			End Get
		End Property

		Protected Friend Overridable ReadOnly Property [Alias]() As String
			Get
				Return MakeAliasFromAssociationPath(associationPath)
			End Get
		End Property

		Protected Friend Overridable ReadOnly Property AliasAndName() As String
			Get
				Return IIf([String].IsNullOrEmpty([Alias]), Name, [Alias] + "." + Name)
			End Get
		End Property

		Protected Friend Overridable ReadOnly Property IsOnRoot() As Boolean
			Get
				Return [String].IsNullOrEmpty([Alias])
			End Get
		End Property

#End Region

#Region "Utility Methods"

		Protected Shared Function MakeAliasFromAssociationPath(ByVal associationPath As String) As String
			If [String].IsNullOrEmpty(associationPath) Then
				Return [String].Empty
			End If
			If associationPath.Equals("this", StringComparison.InvariantCultureIgnoreCase) Then
				Return [String].Empty
			End If
			If associationPath.StartsWith("this.", StringComparison.InvariantCultureIgnoreCase) Then
				Return associationPath.Substring(associationPath.IndexOf(".") + 1)
			End If
			If associationPath.StartsWith(".") Then
				Return associationPath.Substring(1)
			End If
			Return associationPath
		End Function

#End Region
	End Class

#End Region

#Region "QueryClause<T>"

	Public MustInherit Class QueryClause(Of T)
		Inherits QueryPart
#Region "ctor"

		Public Sub New(ByVal name As String, ByVal associationPath As String, ByVal backTrack As Boolean)
			MyBase.New(name, associationPath, backTrack)
		End Sub

		Public Sub New(ByVal name As String, ByVal associationPath As String)
			Me.New(name, associationPath, False)
		End Sub

#End Region

		''' <summary>
		''' Adds a new criteria using the Alias if the criteria does not already exist.
		''' </summary>
		''' <param name="rootCriteria"></param>
		''' <param name="criterias"></param>
		Protected Overridable Sub EnsureCriteriaExistsForAlias(ByVal rootCriteria As DetachedCriteria, ByVal criterias As IDictionary(Of String, DetachedCriteria))
			If backTrack Then
				Return
			End If
			If IsOnRoot Then
				Return
			End If
			If criterias.ContainsKey([Alias]) = False Then
				criterias.Add([Alias], rootCriteria.CreateCriteria([Alias], [Alias]))
			End If
		End Sub

		Protected Shared Function BackTrackAssociationPath(ByVal associationPath As String) As String
			Dim lastIndexOfPeriod As Integer = associationPath.LastIndexOf("."c)
			If lastIndexOfPeriod = -1 Then
				Return associationPath
				'this mean we are on "this", no need to do anything
			End If
			Return associationPath.Substring(0, lastIndexOfPeriod)
		End Function

		Public Function ToDetachedCriteria() As DetachedCriteria
			Return ToDetachedCriteria(Nothing)
		End Function

		Public Overridable Function ToDetachedCriteria(ByVal [alias] As String) As DetachedCriteria
			Dim detachedCriteria As DetachedCriteria
			If [String].IsNullOrEmpty([alias]) Then
				detachedCriteria = DetachedCriteria.[For](GetType(T))
			Else
				detachedCriteria = DetachedCriteria.[For](GetType(T), [alias])
			End If

			Dim criterias As New Dictionary(Of String, DetachedCriteria)()
			BuildQuery(detachedCriteria, criterias)
			Return detachedCriteria
		End Function

		Public Overridable Sub BuildQuery(ByVal rootCriteria As DetachedCriteria, ByVal criterias As IDictionary(Of String, DetachedCriteria))
			EnsureCriteriaExistsForAlias(rootCriteria, criterias)
			AddCriteria(rootCriteria, criterias)
		End Sub

		''' <summary>
		''' 
		''' </summary>
		''' <param name="expr"></param>
		''' <returns></returns>
		''' <remarks>Should be implicit but breaks active record integration with order by clauses.</remarks>
		Public Shared Narrowing Operator CType(ByVal expr As QueryClause(Of T)) As DetachedCriteria
			Return expr.ToDetachedCriteria(Nothing)
		End Operator

		Protected MustOverride Sub AddCriteria(ByVal rootCriteria As DetachedCriteria, ByVal criterias As IDictionary(Of String, DetachedCriteria))
	End Class

#End Region

#Region "SimpleClause<T>"

	Public MustInherit Class SimpleClause(Of T, CT As SimpleClause(Of T, CT))
		Inherits QueryClause(Of T)
#Region "Fields"

		Protected Friend clauses As List(Of CT)

#End Region

#Region "ctor"

		Protected Sub New(ByVal name As String, ByVal associationPath As String, ByVal backTrack As Boolean)
			MyBase.New(name, associationPath, backTrack)
		End Sub

		Protected Sub New(ByVal name As String, ByVal associationPath As String)
			MyBase.New(name, associationPath)
		End Sub

#End Region

		Protected Friend Overridable Sub AddOne(ByVal one As CT)
			If clauses = Nothing Then
				clauses = New List(Of CT)()
			End If
			clauses.Add(one)
			one.clauses = Nothing
		End Sub

		Protected Friend Overridable Sub AddRange(ByVal one As CT)
			If clauses = Nothing Then
				clauses = New List(Of CT)()
			End If
			clauses.AddRange(one.clauses)
			one.clauses = Nothing
		End Sub

		Protected Friend Shared Function Combine(ByVal lhs As CT, ByVal rhs As CT, ByVal newClause As CT) As CT
			If lhs.clauses = Nothing OrElse lhs.clauses.Count = 0 Then
				lhs.AddOne(newClause)
			End If

			If rhs.clauses = Nothing OrElse rhs.clauses.Count = 0 Then
				lhs.AddOne(rhs)
			Else
				lhs.AddRange(rhs)
			End If

			Return lhs
		End Function
	End Class

#End Region

#Region "WhereClause<T>"

	Partial Public Class WhereClause(Of T)
		Inherits QueryClause(Of T)
		Private criterions As ICollection(Of ICriterion) = New List(Of ICriterion)()
		Protected children As New List(Of WhereClause(Of T))()

		Public Sub New(ByVal name As String, ByVal associationPath As String, ByVal backTrackAssociationsOnEquality As Boolean)
			MyBase.New(name, associationPath, backTrackAssociationsOnEquality)
		End Sub

		Public Sub New(ByVal name As String, ByVal associationPath As String)
			MyBase.New(name, associationPath)
		End Sub

		Protected Sub AddCriterion(ByVal criterion As ICriterion)
			criterions.Add(criterion)
		End Sub

		Protected Sub AddChild(ByVal child As WhereClause(Of T))
			If children = Nothing Then
				children = New List(Of WhereClause(Of T))()
			End If
			children.Add(child)
		End Sub

		Protected Overloads Overrides Sub AddCriteria(ByVal rootCriteria As DetachedCriteria, ByVal criterias As IDictionary(Of String, DetachedCriteria))
			For Each criterion As ICriterion In criterions
				If IsOnRoot Then
					rootCriteria.Add(criterion)
				Else
					criterias([Alias]).Add(criterion)
				End If
			Next

			If children <> Nothing Then
				For Each child As WhereClause(Of T) In children
					child.BuildQuery(rootCriteria, criterias)
				Next
			End If
		End Sub

#Region "Expressions"

		Public Function Eq(ByVal value As Object) As WhereClause(Of T)
			Dim eq As AbstractCriterion
			If value = Nothing Then
				eq = Expression.IsNull(name)
			Else
				eq = Expression.Eq(name, value)
			End If
			Dim self As WhereClause(Of T) = Me
			If backTrack Then
				self = New WhereClause(Of T)(name, BackTrackAssociationPath(associationPath))
				AddChild(self)
			End If
			self.AddCriterion(eq)
			Return Me
		End Function


		Public Function NotEq(ByVal value As Object) As WhereClause(Of T)
			Dim eq As AbstractCriterion
			If value = Nothing Then
				eq = Expression.IsNotNull(name)
			Else
				eq = Expression.[Not](Expression.Eq(name, value))
			End If
			Dim self As WhereClause(Of T) = Me
			If backTrack Then
				self = New WhereClause(Of T)(name, BackTrackAssociationPath(associationPath))
				AddChild(self)
			End If
			self.AddCriterion(eq)
			Return Me
		End Function

		Public Function [In](Of K)(ByVal ParamArray values As K()) As WhereClause(Of T)
			Dim inExpression As AbstractCriterion = Expression.[In](name, values)
			Dim self As WhereClause(Of T) = Me
			If backTrack Then
				self = New WhereClause(Of T)(name, BackTrackAssociationPath(associationPath))
				AddChild(self)
			End If
			self.AddCriterion(inExpression)
			Return Me
		End Function

		Public Function [In](ByVal ParamArray values As Object()) As WhereClause(Of T)
			[In](DirectCast(values, ICollection))
			Return Me
		End Function

		Public Function [In](Of K)(ByVal values As ICollection(Of K)) As WhereClause(Of T)
			[In](New List(Of K)(values).ToArray())
			Return Me
		End Function

		Public Function [In](Of K)(ByVal values As IEnumerable(Of K)) As WhereClause(Of T)
			[In](New List(Of K)(values).ToArray())
			Return Me
		End Function

		Public ReadOnly Property IsNotNull() As WhereClause(Of T)
			Get
				Dim notNullExpression As AbstractCriterion = New NotNullExpression(name)
				Dim self As WhereClause(Of T) = Me
				If backTrack Then
					self = New WhereClause(Of T)(name, BackTrackAssociationPath(associationPath))
					AddChild(self)
				End If
				self.AddCriterion(notNullExpression)
				Return Me
			End Get
		End Property

		Public ReadOnly Property IsNull() As WhereClause(Of T)
			Get
				Dim nullExpression As AbstractCriterion = New NullExpression(name)
				Dim self As WhereClause(Of T) = Me
				If backTrack Then
					self = New WhereClause(Of T)(name, BackTrackAssociationPath(associationPath))
					AddChild(self)
				End If
				self.AddCriterion(nullExpression)
				Return Me
			End Get
		End Property

		#End Region Expressions

#Region "Operator Overloading Magic"

		Public Shared Operator =(ByVal expr As WhereClause(Of T), ByVal other As Object) As WhereClause(Of T)
			Return expr.Eq(other)
		End Operator

		Public Shared Operator <>(ByVal expr As WhereClause(Of T), ByVal other As Object) As WhereClause(Of T)
			Return expr.NotEq(other)
		End Operator

		Public Shared Operator And(ByVal lhs As WhereClause(Of T), ByVal rhs As WhereClause(Of T)) As WhereClause(Of T)
			Dim combined As New WhereClause(Of T)(lhs.name, Nothing)
			combined.AddChild(lhs)
			combined.AddChild(rhs)
			Return combined
		End Operator

		Public Shared Operator Not(ByVal other As WhereClause(Of T)) As WhereClause(Of T)
			Dim [not] As New WhereClause(Of T)(other.name, Nothing)
			If other.children = Nothing OrElse other.children.Count <> 0 Then
				Throw New InvalidOperationException("Cannot use ! operator on complex queries")
			End If
			Dim conjunction As New Conjunction()
			For Each crit As ICriterion In other.criterions
				conjunction.Add(crit)
			Next
			[not].AddCriterion(Expression.[Not](conjunction))
			Return [not]
		End Operator

		Public Shared Operator Or(ByVal lhs As WhereClause(Of T), ByVal rhs As WhereClause(Of T)) As WhereClause(Of T)
			If lhs.associationPath <> rhs.associationPath Then
				Throw New InvalidOperationException(String.Format("OR attempted between {0} and {1}." & Chr(13) & "" & Chr(10) & "You can't OR between two Query parts that belong to different associations." & Chr(13) & "" & Chr(10) & "Use HQL for this functionality...", lhs.associationPath, rhs.associationPath))
			End If

			Dim combined As New WhereClause(Of T)(lhs.name, Nothing)
			Dim lhs_conjunction As Conjunction = Expression.Conjunction()
			Dim rhs_conjunction As Conjunction = Expression.Conjunction()
			For Each criterion As ICriterion In lhs.criterions
				lhs_conjunction.Add(criterion)
			Next
			For Each criterion As ICriterion In rhs.criterions
				rhs_conjunction.Add(criterion)
			Next
			combined.criterions.Add(Expression.[Or](lhs_conjunction, rhs_conjunction))
			Return combined
		End Operator

		Public Shared Widening Operator CType(ByVal expr As WhereClause(Of T)) As DetachedCriteria
			Return expr.ToDetachedCriteria(Nothing)
		End Operator

		Public Shared Operator IsTrue(ByVal exp As WhereClause(Of T)) As Boolean
			Return False
		End Operator

		Public Shared Operator IsFalse(ByVal exp As WhereClause(Of T)) As Boolean
			Return False
		End Operator

#End Region

		<Browsable(False)> _
		<Localizable(False)> _
		<Obsolete("You can't use Equals()! Use Eq()", True)> _
		Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
			Throw New InvalidOperationException("You can't use Equals()! Use Eq()")
		End Function

		<Browsable(False)> _
		<Localizable(False)> _
		Public Overloads Overrides Function GetHashCode() As Integer
			Return MyBase.GetHashCode()
		End Function

		Protected Shared Function FromCriterion(ByVal criterion As AbstractCriterion, ByVal name As String, ByVal associationPath As String) As WhereClause(Of T)
			Dim whereClause As New WhereClause(Of T)(name, associationPath)
			whereClause.AddCriterion(criterion)
			Return whereClause
		End Function
	End Class

#End Region

#Region "WhereClauseProperty<T>"

	Partial Public Class WhereClauseProperty(Of T)
		Inherits WhereClause(Of T)
		Public Sub New(ByVal name As String, ByVal associationPath As String)
			MyBase.New(name, associationPath)
		End Sub

#Region "Expressions"

		Public Function Between(ByVal lo As Object, ByVal hi As Object) As WhereClause(Of T)
			Dim betweenExpression As AbstractCriterion = New BetweenExpression(name, lo, hi)
			AddCriterion(betweenExpression)
			Return Me
		End Function

		Public Function EqProperty(ByVal otherPropertyName As String) As WhereClause(Of T)
			Dim eqPropertyExpression As AbstractCriterion = New EqPropertyExpression(name, otherPropertyName)
			AddCriterion(eqPropertyExpression)
			Return Me
		End Function


		Public Function Ge(ByVal value As Object) As WhereClause(Of T)
			Dim geExpression As AbstractCriterion = New GeExpression(name, value)
			AddCriterion(geExpression)
			Return Me
		End Function

		Public Function Gt(ByVal value As Object) As WhereClause(Of T)
			Dim gtExpression As AbstractCriterion = New GtExpression(name, value)
			AddCriterion(gtExpression)
			Return Me
		End Function

		Public Function InsensitiveLike(ByVal value As Object) As WhereClause(Of T)
			Dim insensitiveLikeExpression As AbstractCriterion = New InsensitiveLikeExpression(name, value)
			AddCriterion(insensitiveLikeExpression)
			Return Me
		End Function

		Public Function InsensitiveLike(ByVal value As String, ByVal matchMode As MatchMode) As WhereClause(Of T)
			Dim insensitiveLikeExpression As AbstractCriterion = New InsensitiveLikeExpression(name, value, matchMode)
			AddCriterion(insensitiveLikeExpression)
			Return Me
		End Function

		Public Function Le(ByVal value As Object) As WhereClause(Of T)
			Dim leExpression As AbstractCriterion = New LeExpression(name, value)
			AddCriterion(leExpression)
			Return Me
		End Function

		Public Function LeProperty(ByVal otherPropertyName As String) As WhereClause(Of T)
			Dim lePropertyExpression As AbstractCriterion = New LePropertyExpression(name, otherPropertyName)
			AddCriterion(lePropertyExpression)
			Return Me
		End Function

		Public Function [Like](ByVal value As Object) As WhereClause(Of T)
			Dim likeExpression As AbstractCriterion = New LikeExpression(name, value)
			AddCriterion(likeExpression)
			Return Me
		End Function

		Public Function [Like](ByVal value As String, ByVal matchMode As MatchMode) As WhereClause(Of T)
			Dim likeExpression As AbstractCriterion = New LikeExpression(name, value, matchMode)
			AddCriterion(likeExpression)
			Return Me
		End Function

		Public Function Lt(ByVal value As Object) As WhereClause(Of T)
			Dim ltExpression As AbstractCriterion = New LtExpression(name, value)
			AddCriterion(ltExpression)
			Return Me
		End Function

		Public Function LtProperty(ByVal otherPropertyName As String) As WhereClause(Of T)
			Dim ltPropertyExpression As AbstractCriterion = New LtPropertyExpression(name, otherPropertyName)
			AddCriterion(ltPropertyExpression)
			Return Me
		End Function

#End Region

#Region "Operator Overloading Magic"

		Public Shared Operator >(ByVal expr As WhereClauseProperty(Of T), ByVal other As Object) As WhereClause(Of T)
			Return expr.Gt(other)
		End Operator

		Public Shared Operator <(ByVal expr As WhereClauseProperty(Of T), ByVal other As Object) As WhereClause(Of T)
			Return expr.Lt(other)
		End Operator

		Public Shared Operator >=(ByVal expr As WhereClauseProperty(Of T), ByVal other As Object) As WhereClause(Of T)
			Return expr.Ge(other)
		End Operator

		Public Shared Operator <=(ByVal expr As WhereClauseProperty(Of T), ByVal other As Object) As WhereClause(Of T)
			Return expr.Le(other)
		End Operator

#End Region
	End Class

#End Region

#Region "OrderByEntity<T>"

	Partial Public Class OrderByEntity(Of T)
		Inherits QueryPart
		Public Sub New(ByVal name As String, ByVal associationPath As String)
			MyBase.New(name, associationPath)
		End Sub
	End Class

#End Region

#Region "OrderByClauseProperty<T>"

	Partial Public Class OrderByClauseProperty(Of T)
		Inherits SimpleClause(Of T, OrderByClauseProperty(Of T))
		Protected ascending As Boolean

		Public Sub New(ByVal name As String, ByVal associationPath As String, ByVal backTrack As Boolean)
			Me.New(name, associationPath, backTrack, True)
		End Sub

		Public Sub New(ByVal name As String, ByVal associationPath As String)
			Me.New(name, associationPath, False)
		End Sub

		Public Sub New(ByVal name As String, ByVal associationPath As String, ByVal backTrack As Boolean, ByVal ascending As Boolean)
			MyBase.New(name, associationPath, backTrack)
			Me.ascending = ascending
		End Sub

		Protected Overloads Overrides Sub AddCriteria(ByVal rootCriteria As DetachedCriteria, ByVal criterias As IDictionary(Of String, DetachedCriteria))
			If clauses = Nothing Then
				rootCriteria.AddOrder(Me)
			Else
				For Each o As OrderByClauseProperty(Of T) In clauses
					o.BuildQuery(rootCriteria, criterias)
				Next
			End If
		End Sub

#Region "Expressions"

		Public ReadOnly Property Asc() As OrderByClauseProperty(Of T)
			Get
				ascending = True
				Return Me
			End Get
		End Property

		Public ReadOnly Property Desc() As OrderByClauseProperty(Of T)
			Get
				ascending = False
				Return Me
			End Get
		End Property

#End Region

#Region "Operator Overloading Magic"

		Public Shared Operator And(ByVal lhs As OrderByClauseProperty(Of T), ByVal rhs As OrderByClauseProperty(Of T)) As OrderByClauseProperty(Of T)
			Return Combine(lhs, rhs, New OrderByClauseProperty(Of T)(lhs.name, lhs.associationPath, lhs.backTrack, lhs.ascending))
		End Operator

		Public Shared Operator IsTrue(ByVal exp As OrderByClauseProperty(Of T)) As Boolean
			Return False
		End Operator

		Public Shared Operator IsFalse(ByVal exp As OrderByClauseProperty(Of T)) As Boolean
			Return False
		End Operator

		Public Shared Widening Operator CType(ByVal order As OrderByClauseProperty(Of T)) As Order
			Return New Order(order.AliasAndName, order.ascending)
		End Operator

#End Region
	End Class

#End Region

#Region "ProjectionClauseProperty<T>"

	Public Class ProjectionClauseProperty(Of T)
		Inherits SimpleClause(Of T, ProjectionClauseProperty(Of T))
#Region "Fields"

		Protected Friend hasGrouping As Boolean
		Protected Friend hasDistinct As Boolean
		Protected Friend projection As IProjection

#End Region

#Region "ctor"

		Public Sub New(ByVal name As String, ByVal associationPath As String)
			Me.New(name, associationPath, False)
		End Sub

		Public Sub New(ByVal name As String, ByVal associationPath As String, ByVal backTack As Boolean)
			MyBase.New(name, associationPath, backTack)
		End Sub

		Public Sub New(ByVal name As String, ByVal associationPath As String, ByVal backTack As Boolean, ByVal projection As IProjection)
			MyBase.New(name, associationPath, backTack)
			Me.projection = projection
		End Sub

#End Region

#Region "Projections"

		Public Function ProjectBy() As ProjectionClauseProperty(Of T)
			projection = Projections.[Property](AliasAndName)
			Return Me
		End Function

		Public Function GroupBy() As ProjectionClauseProperty(Of T)
			Return MakeGroup(Projections.GroupProperty(AliasAndName))
		End Function

		Public Function Count() As ProjectionClauseProperty(Of T)
			Return MakeGroup(Projections.Count(AliasAndName))
		End Function

		Public Function CountDistinct() As ProjectionClauseProperty(Of T)
			Return MakeGroup(Projections.CountDistinct(AliasAndName))
		End Function

		Public Function Min() As ProjectionClauseProperty(Of T)
			Return MakeGroup(Projections.Min(AliasAndName))
		End Function

		Public Function Max() As ProjectionClauseProperty(Of T)
			Return MakeGroup(projection)
		End Function

		Protected Function MakeGroup(ByVal groupProjection As IProjection) As ProjectionClauseProperty(Of T)
			projection = groupProjection
			hasGrouping = True
			Return Me
		End Function

#End Region

#Region "QueryClause<T> Overrides"

		Public Overloads Overrides Sub BuildQuery(ByVal rootCriteria As DetachedCriteria, ByVal criterias As IDictionary(Of String, DetachedCriteria))
			If clauses <> Nothing Then
				For Each p As ProjectionClauseProperty(Of T) In clauses
					p.EnsureCriteriaExistsForAlias(rootCriteria, criterias)
				Next
			Else
				EnsureCriteriaExistsForAlias(rootCriteria, criterias)
			End If
			rootCriteria.SetProjection(DirectCast(Me, ProjectionList))
		End Sub

		Protected Overloads Overrides Sub AddCriteria(ByVal rootCriteria As DetachedCriteria, ByVal criterias As IDictionary(Of String, DetachedCriteria))
			' Do nothing here handled in BuildQuery
		End Sub

#End Region

#Region "Operator Overloading Magic"

		Public Shared Widening Operator CType(ByVal expr As ProjectionClauseProperty(Of T)) As ProjectionList
			If expr.hasDistinct Then
				If expr.clauses = Nothing OrElse expr.clauses.Count = 0 Then
					Throw New InvalidOperationException("Cannot create distinct query with zero projections.")
				End If
				Return Projections.ProjectionList().Add(Projections.Distinct(DirectCast(expr, ProjectionList)))
			End If

			If expr.clauses = Nothing OrElse expr.clauses.Count = 0 Then
				Return Projections.ProjectionList().Add(EnsureDefault(expr))
			End If

			Dim list As ProjectionList = Projections.ProjectionList()
			For Each p As ProjectionClauseProperty(Of T) In expr.clauses
				list.Add(EnsureDefault(p))
			Next
			Return list
		End Operator

		Private Shared Function EnsureDefault(ByVal expr As ProjectionClauseProperty(Of T)) As IProjection
			If expr.projection = Nothing Then
				Return IIf(expr.hasGrouping, Projections.GroupProperty(expr.AliasAndName), Projections.[Property](expr.AliasAndName))
			End If
			Return expr.projection
		End Function

		Public Shared Operator And(ByVal lhs As ProjectionClauseProperty(Of T), ByVal rhs As ProjectionClauseProperty(Of T)) As ProjectionClauseProperty(Of T)
			If rhs.hasDistinct Then
				Throw New InvalidOperationException("Cannot add distinct projection to existing projections.")
			End If

			If lhs.hasDistinct AndAlso rhs.hasGrouping Then
				lhs.hasGrouping = True
				Return Combine(ConvertToGrouping(lhs, False), rhs, New ProjectionClauseProperty(Of T)(lhs.name, lhs.associationPath, lhs.backTrack, ConvertToGrouping(lhs.AliasAndName, lhs.projection)))
			End If

			If lhs.hasGrouping AndAlso Not rhs.hasGrouping Then
				Return Combine(lhs, ConvertToGrouping(rhs, True), New ProjectionClauseProperty(Of T)(lhs.name, lhs.associationPath, lhs.backTrack, lhs.projection))
			End If

			If Not lhs.hasGrouping AndAlso rhs.hasGrouping Then
				Return Combine(ConvertToGrouping(lhs, False), rhs, New ProjectionClauseProperty(Of T)(lhs.name, lhs.associationPath, lhs.backTrack, ConvertToGrouping(lhs.AliasAndName, lhs.projection)))
			End If
			Return Combine(lhs, rhs, New ProjectionClauseProperty(Of T)(lhs.name, lhs.associationPath, lhs.backTrack, lhs.projection))
		End Operator

		Friend Shared Function ConvertToGrouping(ByVal aliasAndName As String, ByVal projection As IProjection) As IProjection
			If projection <> Nothing AndAlso projection.IsGrouped Then
				Return projection
			End If

			If projection <> Nothing AndAlso Not (TypeOf projection Is PropertyProjection) Then
				Throw New InvalidOperationException([String].Format("Cannot auto convert {0} type to a GroupPropertyProjection.", projection.[GetType]()))
			End If

			Return Projections.GroupProperty(aliasAndName)
		End Function

		Private Shared Function ConvertToGrouping(ByVal expr As ProjectionClauseProperty(Of T), ByVal addToFront As Boolean) As ProjectionClauseProperty(Of T)
			If expr.clauses = Nothing OrElse expr.clauses.Count = 0 Then
				Return New ProjectionClauseProperty(Of T)(expr.name, expr.associationPath, expr.backTrack, ConvertToGrouping(expr.AliasAndName, expr.projection))
			End If

			Dim converted As ProjectionClauseProperty(Of T)
			If addToFront Then
				converted = New ProjectionClauseProperty(Of T)(expr.Name, expr.associationPath, expr.backTrack, ConvertToGrouping(expr.AliasAndName, expr.projection))
				converted.hasGrouping = True
				converted.AddOne(converted)
			Else
				converted = New ProjectionClauseProperty(Of T)(expr.clauses(0).Name, expr.clauses(0).associationPath, expr.clauses(0).backTrack, ConvertToGrouping(expr.clauses(0).AliasAndName, expr.clauses(0).projection))
				Dim np As New ProjectionClauseProperty(Of T)(converted.Name, converted.associationPath, converted.backTrack, converted.projection)
				np.hasGrouping = True
				converted.AddOne(np)
				converted.hasGrouping = True
				expr.clauses.RemoveAt(0)
			End If

			For Each p As ProjectionClauseProperty(Of T) In expr.clauses
				Dim np As New ProjectionClauseProperty(Of T)(p.name, p.associationPath, p.backTrack, ConvertToGrouping(p.AliasAndName, p.projection))
				np.hasGrouping = True

				p.clauses.Add(np)
			Next
			Return converted
		End Function

		Public Shared Operator IsTrue(ByVal exp As ProjectionClauseProperty(Of T)) As Boolean
			Return False
		End Operator

		Public Shared Operator IsFalse(ByVal exp As ProjectionClauseProperty(Of T)) As Boolean
			Return False
		End Operator

#End Region
	End Class

#End Region

#Region "ProjectionPropertyNumberic<T>"

	Public Class ProjectionClausePropertyNumeric(Of T)
		Inherits ProjectionClauseProperty(Of T)
		Public Sub New(ByVal name As String, ByVal associationPath As String)
			MyBase.New(name, associationPath)
		End Sub

		Public Sub New(ByVal name As String, ByVal associationPath As String, ByVal backTrack As Boolean)
			MyBase.New(name, associationPath, backTrack)
		End Sub

		Public Function Sum() As ProjectionClauseProperty(Of T)
			Return MakeGroup(Projections.Sum(AliasAndName))
		End Function

		Public Function Avg() As ProjectionClauseProperty(Of T)
			Return MakeGroup(Projections.Avg(AliasAndName))
		End Function
	End Class

#End Region

#Region "ProjectionRoot<T>"

	Partial Public Class ProjectionRoot(Of T)
		Public ReadOnly Property RowCount() As ProjectionClauseProperty(Of T)
			Get
				Return New ProjectionClauseProperty(Of T)(Nothing, Nothing, False, Projections.RowCount())
			End Get
		End Property

		Public Function SqlGroupProjection(ByVal sql As String, ByVal groupBy As String, ByVal columnAliases As String(), ByVal types As IType()) As ProjectionClauseProperty(Of T)
			Return New ProjectionClauseProperty(Of T)("this", Nothing, False, Projections.SqlGroupProjection(sql, groupBy, columnAliases, types))
		End Function

		Public Function SqlProjection(ByVal sql As String, ByVal columnAliases As String(), ByVal types As IType()) As ProjectionClauseProperty(Of T)
			Return New ProjectionClauseProperty(Of T)("this", Nothing, False, Projections.SqlProjection(sql, columnAliases, types))
		End Function
	End Class

#End Region

#Region "ProjectionEntity<T>"

	Partial Public Class ProjectionEntity(Of T)
		Inherits QueryPart
		Public Sub New(ByVal name As String, ByVal associationPath As String)
			MyBase.New(name, associationPath)
		End Sub

		Public Sub New(ByVal name As String, ByVal associationPath As String, ByVal backTrack As Boolean)
			MyBase.New(name, associationPath, backTrack)
		End Sub
	End Class

#End Region

#Region "SubQuery"

	Public Class Subquery
		Private subQuery As DetachedCriteria

		Friend Sub New(ByVal subQuery As DetachedCriteria)
			Me.subQuery = subQuery
		End Sub

		Public Shared Widening Operator CType(ByVal subquery As Subquery) As DetachedCriteria
			Return subquery.subQuery
		End Operator
	End Class

#End Region
End Namespace
