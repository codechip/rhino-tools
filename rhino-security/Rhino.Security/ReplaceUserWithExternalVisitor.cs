namespace Rhino.Security
{
    using System;
    using System.Collections.Generic;
    using Castle.ActiveRecord;
    using Castle.ActiveRecord.Framework.Internal;

    internal class ReplaceUserWithExternalVisitor<TExternalUser> : AbstractDepthFirstVisitor
        where TExternalUser : IExternalUser, new()
    {
        private ActiveRecordModel activeRecordModel;
        readonly List<BelongsToModel> removableBelongsTo = new List<BelongsToModel>();

        public override void VisitModel(ActiveRecordModel model)
        {
            this.activeRecordModel = model;
            base.VisitModel(model);
            foreach (BelongsToModel belongsToModel in removableBelongsTo)
            {
                model.BelongsTo.Remove(belongsToModel);
            }
        }

        public override void VisitBelongsTo(BelongsToModel model)
        {
            if (model.Property.PropertyType != typeof(IUser))
                return;

            PropertyAttribute att = new PropertyAttribute(model.BelongsToAtt.Column);

            att.Column = model.BelongsToAtt.Column;
            att.Access = model.BelongsToAtt.Access;
            att.ColumnType = typeof(ExternalUserType<TExternalUser>).AssemblyQualifiedName;
            att.Unique = model.BelongsToAtt.Unique;
            att.UniqueKey = model.BelongsToAtt.UniqueKey;
            att.Update = model.BelongsToAtt.Update;
            att.Insert = model.BelongsToAtt.Insert;


            PropertyModel propertyModel = new PropertyModel(model.Property, att);
            activeRecordModel.Properties.Add(propertyModel);
            removableBelongsTo.Add(model);
        }

        public override void VisitHasAndBelongsToMany(HasAndBelongsToManyModel model)
        {
            if (model.HasManyAtt.MapType != typeof(IUser))
                return;
            model.HasManyAtt.Element = model.HasManyAtt.ColumnRef;
            model.HasManyAtt.ColumnRef = null;
            model.HasManyAtt.MapType = typeof(ExternalUserType<TExternalUser>);
            model.HasManyAtt.ElementType = typeof (ExternalUserType<TExternalUser>).AssemblyQualifiedName;
        }

        public override void VisitHasMany(HasManyModel model)
        {
            if (model.HasManyAtt.MapType != typeof(IUser))
                return;
            throw new NotImplementedException(model.Property.ToString());
        }
    }
}