Imports NHibernate.Criterion
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel

Namespace Query
	Partial Public Class OrderByClause
		' Methods
		Public Sub New(ByVal name As String)
			Me.ascending = True
			Me.name = name
		End Sub

		Public Shared Widening Operator CType(ByVal order As OrderByClause) As Order
			Return New Order(order.name, order.ascending)
		End Operator


		' Properties
		Public ReadOnly Property Asc() As OrderByClause
			Get
				Me.ascending = True
				Return Me
			End Get
		End Property

		Public ReadOnly Property Desc() As OrderByClause
			Get
				Me.ascending = False
				Return Me
			End Get
		End Property


		' Fields
		Private ascending As Boolean
		Private name As String
	End Class

	Partial Public Class PropertyQueryBuilder(Of T)
		Inherits QueryBuilder(Of T)

		' Methods
		Public Sub New(ByVal name As String, ByVal associationPath As String)
			MyBase.New(name, associationPath)
		End Sub

		Public Function Between(ByVal lo As Object, ByVal hi As Object) As QueryBuilder(Of T)
			Dim criterion1 As AbstractCriterion = New BetweenExpression(MyBase.name, lo, hi)
			MyBase.AddCriterion(criterion1)
			Return Me
		End Function

		Public Function EqProperty(ByVal otherPropertyName As String) As QueryBuilder(Of T)
			Dim criterion1 As AbstractCriterion = New EqPropertyExpression(MyBase.name, otherPropertyName)
			MyBase.AddCriterion(criterion1)
			Return Me
		End Function

		Public Function Ge(ByVal value As Object) As QueryBuilder(Of T)
			Dim criterion1 As AbstractCriterion = New GeExpression(MyBase.name, value)
			MyBase.AddCriterion(criterion1)
			Return Me
		End Function

		Public Function Gt(ByVal value As Object) As QueryBuilder(Of T)
			Dim criterion1 As AbstractCriterion = New GtExpression(MyBase.name, value)
			MyBase.AddCriterion(criterion1)
			Return Me
		End Function

		Public Function InsensitiveLike(ByVal value As Object) As QueryBuilder(Of T)
			Dim criterion1 As AbstractCriterion = New InsensitiveLikeExpression(MyBase.name, value)
			MyBase.AddCriterion(criterion1)
			Return Me
		End Function

		Public Function InsensitiveLike(ByVal value As String, ByVal matchMode As MatchMode) As QueryBuilder(Of T)
			Dim criterion1 As AbstractCriterion = New InsensitiveLikeExpression(MyBase.name, value, matchMode)
			MyBase.AddCriterion(criterion1)
			Return Me
		End Function

		Public Function Le(ByVal value As Object) As QueryBuilder(Of T)
			Dim criterion1 As AbstractCriterion = New LeExpression(MyBase.name, value)
			MyBase.AddCriterion(criterion1)
			Return Me
		End Function

		Public Function LeProperty(ByVal otherPropertyName As String) As QueryBuilder(Of T)
			Dim criterion1 As AbstractCriterion = New LePropertyExpression(MyBase.name, otherPropertyName)
			MyBase.AddCriterion(criterion1)
			Return Me
		End Function

		Public Function [Like](ByVal value As Object) As QueryBuilder(Of T)
			Dim criterion1 As AbstractCriterion = New LikeExpression(MyBase.name, value)
			MyBase.AddCriterion(criterion1)
			Return Me
		End Function

		Public Function [Like](ByVal value As String, ByVal matchMode As MatchMode) As QueryBuilder(Of T)
			Dim criterion1 As AbstractCriterion = New LikeExpression(MyBase.name, value, matchMode)
			MyBase.AddCriterion(criterion1)
			Return Me
		End Function

		Public Function Lt(ByVal value As Object) As QueryBuilder(Of T)
			Dim criterion1 As AbstractCriterion = New LtExpression(MyBase.name, value)
			MyBase.AddCriterion(criterion1)
			Return Me
		End Function

		Public Function LtProperty(ByVal otherPropertyName As String) As QueryBuilder(Of T)
			Dim criterion1 As AbstractCriterion = New LtPropertyExpression(MyBase.name, otherPropertyName)
			MyBase.AddCriterion(criterion1)
			Return Me
		End Function

		Public Shared Operator >(ByVal expr As PropertyQueryBuilder(Of T), ByVal other As Object) As QueryBuilder(Of T)
			Return expr.Gt(other)
		End Operator

		Public Shared Operator >=(ByVal expr As PropertyQueryBuilder(Of T), ByVal other As Object) As QueryBuilder(Of T)
			Return expr.Ge(other)
		End Operator

		Public Shared Operator <(ByVal expr As PropertyQueryBuilder(Of T), ByVal other As Object) As QueryBuilder(Of T)
			Return expr.Lt(other)
		End Operator

		Public Shared Operator <=(ByVal expr As PropertyQueryBuilder(Of T), ByVal other As Object) As QueryBuilder(Of T)
			Return expr.Le(other)
		End Operator

	End Class

	Partial Public Class QueryBuilder(Of T)
		' Methods
		Public Sub New(ByVal name As String, ByVal associationPath As String)
			Me.children = New List(Of QueryBuilder(Of T))
			Me.criterions = New List(Of ICriterion)
			Me.name = name
			Me.associationPath = IIf(associationPath <> Nothing, associationPath, "this")
		End Sub

		Public Sub New(ByVal name As String, ByVal associationPath As String, ByVal backTrackAssociationsOnEquality As Boolean)
			Me.children = New List(Of QueryBuilder(Of T))
			Me.criterions = New List(Of ICriterion)
			Me.name = name
			Me.associationPath = associationPath
			Me.backTrackAssociationsOnEquality = backTrackAssociationsOnEquality
		End Sub

		Private Sub AddByAssociationPath(ByVal criterionsByAssociation As IDictionary(Of String, ICollection(Of ICriterion)))
			If Not criterionsByAssociation.ContainsKey(Me.associationPath) Then
				criterionsByAssociation.Add(Me.associationPath, DirectCast(New List(Of ICriterion), ICollection(Of ICriterion)))
			End If
			Dim criterion1 As ICriterion
			For Each criterion1 In Me.criterions
				criterionsByAssociation.Item(Me.associationPath).Add(criterion1)
			Next
			Dim builder1 As QueryBuilder(Of T)
			For Each builder1 In Me.children
				builder1.AddByAssociationPath(criterionsByAssociation)
			Next
		End Sub

		Protected Sub AddCriterion(ByVal criterion As AbstractCriterion)
			Me.criterions.Add(criterion)
		End Sub

		Protected Shared Function BackTrackAssociationPath(ByVal associationPath As String) As String
			Dim num1 As Integer = associationPath.LastIndexOf("."c)
			If (num1 = -1) Then
				Return associationPath
			End If
			Return associationPath.Substring(0, num1)
		End Function

		Public Function Eq(ByVal value As Object) As QueryBuilder(Of T)
			Dim criterion1 As AbstractCriterion
			If (value Is Nothing) Then
				criterion1 = Expression.IsNull(Me.name)
			Else
				criterion1 = Expression.Eq(Me.name, value)
			End If
			Dim builder1 As QueryBuilder(Of T) = Me
			If Me.backTrackAssociationsOnEquality Then
				builder1 = New QueryBuilder(Of T)(Me.name, QueryBuilder(Of T).BackTrackAssociationPath(Me.associationPath))
				Me.children.Add(DirectCast(builder1, QueryBuilder(Of T)))
			End If
			builder1.AddCriterion(criterion1)
			Return Me
		End Function

		<Browsable(False), Localizable(False)> _
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Throw New InvalidOperationException("You can't use Equals()! Use Eq()")
		End Function

		<Localizable(False), Browsable(False)> _
		Public Overrides Function GetHashCode() As Integer
			Return MyBase.GetHashCode
		End Function

		Public Function [In](Of K)(ByVal values As ICollection(Of K)) As QueryBuilder(Of T)
			Dim objArray1 As Object() = QueryBuilder(Of T).ToArray(Of K)(values)
			Dim criterion1 As AbstractCriterion = New InExpression(Me.name, objArray1)
			Me.AddCriterion(criterion1)
			Return Me
		End Function

		Public Function [In](ByVal ParamArray values As Object()) As QueryBuilder(Of T)
			Dim criterion1 As AbstractCriterion = New InExpression(Me.name, values)
			Me.AddCriterion(criterion1)
			Return Me
		End Function

		Public Function [In](ByVal values As ICollection) As QueryBuilder(Of T)
			Dim criterion1 As AbstractCriterion = New InExpression(Me.name, QueryBuilder(Of T).ToArray(values))
			Me.AddCriterion(criterion1)
			Return Me
		End Function

		Public Function NotEq(ByVal value As Object) As QueryBuilder(Of T)
			Dim criterion1 As AbstractCriterion
			If (value Is Nothing) Then
				criterion1 = Expression.IsNotNull(Me.name)
			Else
				criterion1 = Expression.Not(Expression.Eq(Me.name, value))
			End If
			Dim builder1 As QueryBuilder(Of T) = Me
			If Me.backTrackAssociationsOnEquality Then
				builder1 = New QueryBuilder(Of T)(Me.name, QueryBuilder(Of T).BackTrackAssociationPath(Me.associationPath))
				Me.children.Add(DirectCast(builder1, QueryBuilder(Of T)))
			End If
			builder1.AddCriterion(criterion1)
			Return Me
		End Function

		Public Shared Operator And(ByVal lhs As QueryBuilder(Of T), ByVal rhs As QueryBuilder(Of T)) As QueryBuilder(Of T)
			Dim builder1 As New QueryBuilder(Of T)(lhs.name, Nothing)
			builder1.children.Add(DirectCast(lhs, QueryBuilder(Of T)))
			builder1.children.Add(DirectCast(rhs, QueryBuilder(Of T)))
			Return builder1
		End Operator

		Public Shared Operator Or(ByVal lhs As QueryBuilder(Of T), ByVal rhs As QueryBuilder(Of T)) As QueryBuilder(Of T)
			Dim criterion1 As ICriterion
			Dim enumerator1 As IEnumerator(Of ICriterion)
			If (Not lhs.associationPath Is rhs.associationPath) Then
				Throw New InvalidOperationException(String.Format("OR attempted between {0} and {1}." & ChrW(13) & ChrW(10) & "You can't OR between two Query parts that belong to different associations." & ChrW(13) & ChrW(10) & "Use HQL for this functionality...", lhs.associationPath, rhs.associationPath))
			End If
			Dim builder1 As New QueryBuilder(Of T)(lhs.name, Nothing)
			Dim conjunction1 As Conjunction = Expression.Conjunction
			Dim conjunction2 As Conjunction = Expression.Conjunction
			Using enumerator1 = lhs.criterions.GetEnumerator
				Do While enumerator1.MoveNext
					criterion1 = enumerator1.Current
					conjunction1.Add(criterion1)
				Loop
			End Using
			Using enumerator1 = rhs.criterions.GetEnumerator
				Do While enumerator1.MoveNext
					criterion1 = enumerator1.Current
					conjunction2.Add(criterion1)
				Loop
			End Using
			builder1.criterions.Add(Expression.Or(conjunction1, conjunction2))
			Return builder1
		End Operator

		Public Shared Operator =(ByVal expr As QueryBuilder(Of T), ByVal other As Object) As QueryBuilder(Of T)
			Return expr.Eq(other)
		End Operator

		Public Shared Operator IsFalse(ByVal exp As QueryBuilder(Of T)) As Boolean
			Return False
		End Operator

		Public Shared Widening Operator CType(ByVal expr As QueryBuilder(Of T)) As DetachedCriteria
			Dim criteria1 As DetachedCriteria = DetachedCriteria.For(Of T)()
			Dim dictionary1 As New Dictionary(Of String, ICollection(Of ICriterion))
			expr.AddByAssociationPath(dictionary1)
			Dim pair1 As KeyValuePair(Of String, ICollection(Of ICriterion))
			For Each pair1 In dictionary1
				Dim criteria2 As DetachedCriteria = criteria1
				If (Not pair1.Key Is "this") Then
					criteria2 = criteria1.CreateCriteria(pair1.Key)
				End If
				Dim criterion1 As ICriterion
				For Each criterion1 In pair1.Value
					criteria2.Add(criterion1)
				Next
			Next
			Return criteria1
		End Operator

		Public Shared Operator <>(ByVal expr As QueryBuilder(Of T), ByVal other As Object) As QueryBuilder(Of T)
			Return expr.NotEq(other)
		End Operator

		Public Shared Operator IsTrue(ByVal exp As QueryBuilder(Of T)) As Boolean
			Return False
		End Operator

		Protected Shared Function ToArray(Of K)(ByVal values As ICollection(Of K)) As Object()
			Dim localArray1 As K() = New K(values.Count - 1) {}
			values.CopyTo(localArray1, 0)
			Return QueryBuilder(Of T).ToArray(localArray1)
		End Function

		Protected Shared Function ToArray(ByVal values As ICollection) As Object()
			Dim objArray1 As Object() = New Object(values.Count - 1) {}
			values.CopyTo(objArray1, 0)
			Return objArray1
		End Function


		' Properties
		Public ReadOnly Property IsNotNull() As QueryBuilder(Of T)
			Get
				Dim criterion1 As AbstractCriterion = New NotNullExpression(Me.name)
				Me.AddCriterion(criterion1)
				Return Me
			End Get
		End Property

		Public ReadOnly Property IsNull() As QueryBuilder(Of T)
			Get
				Dim criterion1 As AbstractCriterion = New NullExpression(Me.name)
				Me.AddCriterion(criterion1)
				Return Me
			End Get
		End Property


		' Fields
		Protected associationPath As String
		Private backTrackAssociationsOnEquality As Boolean
		Private children As ICollection(Of QueryBuilder(Of T))
		Private criterions As ICollection(Of ICriterion)
		Protected name As String
	End Class

	Partial Public Class ProjectBy
		Public Shared ReadOnly Property RowCount() As IProjection
			Get
				Return Projections.RowCount()
			End Get
		End Property

		Public Shared ReadOnly Property Id() As IProjection
			Get
				Return Projections.Id()
			End Get
		End Property

		Public Shared Function Distinct(ByVal projection As IProjection) As IProjection
			Return Projections.Distinct(projection)
		End Function


		Public Shared Function SqlProjection(ByVal sql As String, ByVal aliases() As String, ByVal types() As IType) As IProjection
			Return Projections.SqlProjection(sql, aliases, types)
		End Function


		Public Shared Function SqlGroupByProjection(ByVal sql As String, ByVal groupBy As String, ByVal aliases() As String, ByVal types() As IType) As IProjection
			Return Projections.SqlGroupProjection(sql, groupBy, aliases, types)
		End Function
	End Class
 _
	Partial Public Class PropertyProjectionBuilder
		Protected name As String


		Public Sub New(ByVal name As String)
			Me.name = name
		End Sub


		Public ReadOnly Property Count() As IProjection
			Get
				Return Projections.Count(name)
			End Get
		End Property

		Public ReadOnly Property DistinctCount() As IProjection
			Get
				Return Projections.CountDistinct(name)
			End Get
		End Property

		Public ReadOnly Property Max() As IProjection
			Get
				Return Projections.Max(name)
			End Get
		End Property

		Public ReadOnly Property Min() As IProjection
			Get
				Return Projections.Min(name)
			End Get
		End Property

		Public Shared Widening Operator CType(ByVal projection As PropertyProjectionBuilder) As PropertyProjection
			Return Projections.Property(projection.name)
		End Operator

	End Class
 _

	Partial Public Class NumericPropertyProjectionBuilder
		Inherits PropertyProjectionBuilder

		Public Sub New(ByVal name As String)
			MyBase.New(name)
		End Sub

		Public ReadOnly Property Avg() As IProjection
			Get
				Return Projections.Avg(name)
			End Get
		End Property

		Public ReadOnly Property Sum() As IProjection
			Get
				Return Projections.Sum(name)
			End Get
		End Property

		Public Shared Widening Operator CType(ByVal projection As NumericPropertyProjectionBuilder) As PropertyProjection
			Return Projections.Property(projection.name)
		End Operator
	End Class

End Namespace





