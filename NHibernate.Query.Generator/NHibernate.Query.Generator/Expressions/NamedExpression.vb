Imports NHibernate.Expression
Imports System
Imports System.Collections
Imports System.Collections.Generic

Namespace Query
    Public Class NamedExpression
        ' Methods
        Public Sub New(ByVal name As String)
            Me.name = name
        End Sub

        Public Function Between(ByVal lo As Object, ByVal hi As Object) As AbstractCriterion
            Return New BetweenExpression(Me.name, lo, hi)
        End Function

        Public Function Eq(ByVal value As Object) As AbstractCriterion
            Return Expression.Eq(Me.name, value)
        End Function

        Public Function EqProperty(ByVal otherPropertyName As String) As AbstractCriterion
            Return New EqPropertyExpression(Me.name, otherPropertyName)
        End Function

        Public Function Ge(ByVal value As Object) As SimpleExpression
            Return New GeExpression(Me.name, value)
        End Function

        Public Function Gt(ByVal value As Object) As SimpleExpression
            Return New GtExpression(Me.name, value)
        End Function

        Public Function [In](Of T)(ByVal values As ICollection(Of T)) As AbstractCriterion
            Dim objArray1 As Object() = NamedExpression.ToArrayGeneric(Of T)(values)
            Return New InExpression(Me.name, objArray1)
        End Function

        Public Function [In](ByVal ParamArray values As Object()) As AbstractCriterion
            Return New InExpression(Me.name, values)
        End Function

        Public Function [In](ByVal values As ICollection) As AbstractCriterion
            Return New InExpression(Me.name, NamedExpression.ToArray(values))
        End Function

        Public Function InsensitiveLike(ByVal value As Object) As AbstractCriterion
            Return New InsensitiveLikeExpression(Me.name, value)
        End Function

        Public Function InsensitiveLike(ByVal value As String, ByVal matchMode As MatchMode) As AbstractCriterion
            Return New InsensitiveLikeExpression(Me.name, value, matchMode)
        End Function

        Public Function IsNotNull() As AbstractCriterion
            Return New NotNullExpression(Me.name)
        End Function

        Public Function IsNull() As AbstractCriterion
            Return New NullExpression(Me.name)
        End Function

        Public Function Le(ByVal value As Object) As SimpleExpression
            Return New LeExpression(Me.name, value)
        End Function

        Public Function LeProperty(ByVal otherPropertyName As String) As AbstractCriterion
            Return New LePropertyExpression(Me.name, otherPropertyName)
        End Function

        Public Function [Like](ByVal value As Object) As SimpleExpression
            Return New LikeExpression(Me.name, value)
        End Function

        Public Function [Like](ByVal value As String, ByVal matchMode As MatchMode) As SimpleExpression
            Return New LikeExpression(Me.name, value, matchMode)
        End Function

        Public Function Lt(ByVal value As Object) As SimpleExpression
            Return New LtExpression(Me.name, value)
        End Function

        Public Function LtProperty(ByVal otherPropertyName As String) As AbstractCriterion
            Return New LtPropertyExpression(Me.name, otherPropertyName)
        End Function

        Public Shared Operator =(ByVal expr As NamedExpression, ByVal other As Object) As AbstractCriterion
            Return expr.Eq(other)
        End Function

        Public Shared Operator >(ByVal expr As NamedExpression, ByVal other As Object) As AbstractCriterion
            Return expr.Gt(other)
        End Function

        Public Shared Operator >=(ByVal expr As NamedExpression, ByVal other As Object) As AbstractCriterion
            Return expr.Ge(other)
        End Function

        Public Shared Operator <>(ByVal expr As NamedExpression, ByVal other As Object) As AbstractCriterion
            Return New NotExpression(expr.Eq(other))
        End Function

        Public Shared Operator <(ByVal expr As NamedExpression, ByVal other As Object) As AbstractCriterion
            Return expr.Lt(other)
        End Function

        Public Shared Operator <=(ByVal expr As NamedExpression, ByVal other As Object) As AbstractCriterion
            Return expr.Le(other)
        End Function

        Private Shared Function ToArray(ByVal values As ICollection) As Object()
            Dim objArray1 As Object() = New Object(values.Count  - 1) {}
            values.CopyTo(objArray1, 0)
            Return objArray1
        End Function

        Private Shared Function ToArrayGeneric(Of T)(ByVal values As ICollection(Of T)) As Object()
            Dim localArray1 As T() = New T(values.Count  - 1) {}
            values.CopyTo(localArray1, 0)
            Return NamedExpression.ToArray(localArray1)
        End Function


		<System.ComponentModel.Browsable(False)> _
		<System.ComponentModel.Localizable(False)> _
		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Throw New System.InvalidOperationException("You can't use Equals()! Use Eq()")
		End Function

		<System.ComponentModel.Browsable(False)> _
		<System.ComponentModel.Localizable(False)> _
		Public Overrides Function GetHashCode() As Integer
			Return MyBase.GetHashCode()
		End Function


        ' Fields
        Private name As String
    End Class
End Namespace


