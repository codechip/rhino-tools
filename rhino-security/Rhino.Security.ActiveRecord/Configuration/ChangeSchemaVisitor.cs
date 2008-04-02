using Castle.ActiveRecord.Framework.Internal;
using Rhino.Security.Configuration;

namespace Rhino.Security.AR.Configuration
{
  internal class ChangeSchemaVisitor : AbstractDepthFirstVisitor
  {
    private readonly SecurityTableStructure tableStructure;

    public ChangeSchemaVisitor(SecurityTableStructure tableStructure)
    {
      this.tableStructure = tableStructure;
    }

    public override void VisitModel(ActiveRecordModel model)
    {
      if (tableStructure == SecurityTableStructure.Schema)
        model.ActiveRecordAtt.Schema = "security";
      else if (model.ActiveRecordAtt.Table.StartsWith("security_") == false)
        model.ActiveRecordAtt.Table = "security_" + model.ActiveRecordAtt.Table;

      base.VisitModel(model);
    }

    public override void VisitHasAndBelongsToMany(HasAndBelongsToManyModel model)
    {
      if (tableStructure == SecurityTableStructure.Schema)
      {
        model.HasManyAtt.Schema = "security";
      }
      else if (model.HasManyAtt.Table.StartsWith("security_") == false)
      {
        model.HasManyAtt.Table = "security_" + model.HasManyAtt.Table;
      }
    }
  }
}