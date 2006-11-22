using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.CSharp;

namespace XsdToSchemaUICode
{
	/// <summary>
	/// Summary description for XsdToSchemaUICode.
	/// </summary>
	public class XsdToSchemaUICode
	{
		public static void Main(string[] args)
		{
			if (args.Length != 2 || !File.Exists(args[0]))
			{
				PrintUsage();
				return;
			}
			new XsdToSchemaUICode().Run(args[0], args[1]);
		}

		public void Run(string src, string dest)
		{
			// Load the schema to process.
			XmlSchema xsd;
			using (Stream stm = File.OpenRead(src))
				xsd = XmlSchema.Read(stm, null);
			// Collection of schemas for the XmlSchemaImporter
			XmlSchemas xsds = new XmlSchemas();
			xsds.Add(xsd);
			XmlSchemaImporter imp = new XmlSchemaImporter(xsds);
	
			// System.CodeDom namespace for the XmlCodeExporter to put classes in
			CodeNamespace ns = new CodeNamespace("NHibernate.Mapping.Hbm");
			CodeCompileUnit ccu =  new CodeCompileUnit();
			XmlCodeExporter exp = new XmlCodeExporter(ns, ccu, ~CodeGenerationOptions.GenerateProperties);
			
			// Iterate schema items (top-level elements only) and generate code for each
			foreach (XmlSchemaObject item in xsd.Items)
			{
				if (item is XmlSchemaElement)
				{
					// Import the mapping first
					XmlTypeMapping map = imp.ImportTypeMapping(new XmlQualifiedName(((XmlSchemaElement)item).Name, xsd.TargetNamespace));
					// Export the code finally
					exp.ExportTypeMapping(map);
				}
			}
			ns.Imports.Add(new CodeNamespaceImport("Ayende.NHibernateQueryAnalyzer.SchemaEditing"));
			AddRequiredTags(ns, xsd);

			// Code generator to build code with.
			ICodeGenerator generator = new CSharpCodeProvider().CreateGenerator();

			// Generate untouched version
			using (StreamWriter sw = new StreamWriter(dest, false))
				generator.GenerateCodeFromNamespace(ns, sw, new CodeGeneratorOptions());
		}

		/// <summary>
		/// Adds the required tags.
		/// It walks the xsd parallel to the ns and add RequiredTag(MinimumAmount) as needed).
		/// </summary>
		private void AddRequiredTags(CodeNamespace ns, XmlSchema xsd)
		{
			foreach (XmlSchemaObject item in xsd.Items)
			{
				if (item is XmlSchemaElement)
				{
					XmlSchemaElement e = (XmlSchemaElement)item;
					ProcessElement(e, "", ns, false, xsd);
				}
			}
		}

		private void ProcessElement(XmlSchemaElement e, string parent, CodeNamespace ns, bool parentOptional, XmlSchema xsd)
		{
			string name = e.Name != null ? e.Name : e.RefName.Name;

			if (!parentOptional && e.MinOccurs > 0 && parent != "")
			{
				AddRequiredTag(ns, parent, name, (int)e.MinOccurs);
			}

			if (e.SchemaType is XmlSchemaComplexType)
			{
				XmlSchemaComplexType complex = (XmlSchemaComplexType)e.SchemaType;
				ProcessComplexType(complex.Particle, ns, name, e.MinOccurs == 0, xsd);
				foreach (XmlSchemaObject o in complex.Attributes)
				{
					if (o is XmlSchemaAttributeGroupRef)
						ProcessGroupRef((XmlSchemaAttributeGroupRef)o, name, ns, xsd);
					else if (o is XmlSchemaAttribute)
						ProcessAttribute((XmlSchemaAttribute)o, name, ns);
					else
						throw new InvalidOperationException("Don't know how to handle: " + o.GetType());
				}
			}
		}

		private void ProcessComplexType(XmlSchemaObject particle, CodeNamespace ns, string name, bool parentOptional, XmlSchema xsd)
		{
			if (particle is XmlSchemaElement)
			{
				XmlSchemaElement e = (XmlSchemaElement)particle;
				ProcessElement(e, name, ns, parentOptional, xsd);
			}
			else if (particle is XmlSchemaAll)
			{
				XmlSchemaAll a = (XmlSchemaAll)particle;
				foreach (XmlSchemaObject o in a.Items)
				{
					if (o is XmlSchemaElement)
						ProcessElement((XmlSchemaElement)o, name, ns, a.MinOccurs == 0, xsd);
				}
			}
			else if (particle is XmlSchemaSequence)
			{
				XmlSchemaSequence c = (XmlSchemaSequence)particle;
				foreach (XmlSchemaObject o in c.Items)
					ProcessComplexType(o, ns, name, c.MinOccurs == 0, xsd);
			}
			else if (particle is XmlSchemaChoice)
			{
				XmlSchemaChoice c = (XmlSchemaChoice)particle;
				foreach (XmlSchemaObject o in c.Items)
					ProcessComplexType(o, ns, name, c.MinOccurs == 0, xsd);
			}
		}

		private void ProcessAttribute(XmlSchemaAttribute a, string parent, CodeNamespace ns)
		{
			if (a.Use == XmlSchemaUse.Required)
			{
				int minAmount = 1;
				AddRequiredTag(ns, parent, a.Name, minAmount);
			}
		}

		private void AddRequiredTag(CodeNamespace ns, string className, string fieldName, int minAmount)
		{
			CodeTypeDeclaration type = FindType(ns, className);
			CodeMemberField field = FindField(type, fieldName);
			foreach (CodeAttributeDeclaration customAttribute in field.CustomAttributes)
			{
				//Avoid duplicate requiredtag attributes
				if (customAttribute.Name == "RequiredTag")
				{
					CodePrimitiveExpression cpe = (CodePrimitiveExpression)customAttribute.Arguments[0].Value;
					int min = (int)cpe.Value;
					//If there is already an attribute, but with a lower value, increase the value.
					if (minAmount > min)
						cpe.Value = minAmount;
					return;
				}
			}
			CodeAttributeDeclaration att = new CodeAttributeDeclaration("RequiredTag");
			att.Arguments.Add(new CodeAttributeArgument("MinimumAmount", new CodePrimitiveExpression(minAmount)));
			field.CustomAttributes.Add(att);
		}

		private CodeTypeDeclaration FindType(CodeNamespace ns, string name)
		{
			string strippedName = name.Replace("-", "");
			foreach (CodeTypeDeclaration type in ns.Types)
			{
				if (type.Name == strippedName)
					return type;
			}
			throw new InvalidOperationException("Unable to find type");
		}

		private CodeMemberField FindField(CodeTypeDeclaration type, string name)
		{
			string strippedName = name.Replace("-", "");
			foreach (CodeTypeMember member in type.Members)
			{
				if (member is CodeMemberField)
				{
					if (member.Name == strippedName || HasXmlAttributeWithName(member, name))
						return (CodeMemberField)member;
				}
			}
			throw new InvalidOperationException("Unable to find field: "+name);
		}

		private static bool HasXmlAttributeWithName(CodeTypeMember member, string name)
		{
			foreach (CodeAttributeDeclaration att in member.CustomAttributes)
			{
				if (att.Name == "System.Xml.Serialization.XmlElementAttribute")
				{
					foreach (CodeAttributeArgument argument in att.Arguments)
					{
						if (argument.Name == "" && argument.Value is CodePrimitiveExpression)
						{
							CodePrimitiveExpression cpe = (CodePrimitiveExpression)argument.Value;
							if (((string)cpe.Value) == name)
								return true;
						}
					}
				}
			}
			return false;
		}

		private static void PrintUsage()
		{
			Console.WriteLine(@"XsdToSchemaUI usage: XsdToSchemaUI <xsd file> <destination file>");
		}

		private void ProcessGroupRef(XmlSchemaAttributeGroupRef groupRef, string parent, CodeNamespace ns, XmlSchema xsd)
		{
			foreach (XmlSchemaAttribute att in ((XmlSchemaAttributeGroup)xsd.AttributeGroups[groupRef.RefName]).Attributes)
			{
				ProcessAttribute(att, parent, ns);
			}
		}
	}
}