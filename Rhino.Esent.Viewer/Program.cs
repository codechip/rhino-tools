namespace Rhino.Esent.Viewer
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using Microsoft.Isam.Esent.Interop;

	internal class Program
	{
		private static void Main(string[] args)
		{
			try
			{
				string database = @"E:\temp\Rhino.DivanDB\Rhino.DivanDB.Tests\bin\Debug\test.divan";

				using (var instance = new Instance("viewer"))
				{
					instance.Init();

					using (var session = new Session(instance.JetInstance))
					{
						Api.JetAttachDatabase(session.JetSesid, database,
						                      AttachDatabaseGrbit.ReadOnly);

						JET_DBID dbid;
						Api.JetOpenDatabase(session.JetSesid, database, "", out dbid, OpenDatabaseGrbit.ReadOnly);

						foreach (string name in Api.GetTableNames(session.JetSesid, dbid))
						{
							Console.WriteLine(name);

							using (var table = new Table(session.JetSesid, dbid, name, OpenTableGrbit.ReadOnly))
							{
								var dictionary = Api.GetTableColumns(session.JetSesid, table.JetTableid);

								if(Api.TryMoveFirst(session.JetSesid, table.JetTableid)==false)
									continue;
								do
								{
									OutputRow(session, table, dictionary);
									Console.WriteLine("=-=-=-=-=-=-");
								} while (Api.TryMoveNext(session.JetSesid, table.JetTableid));
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}

		private static void OutputRow(Session session, Table table, IEnumerable<ColumnInfo> dictionary)
		{
			foreach (ColumnInfo pair in dictionary)
			{
				Console.Write("{0} :",pair.Name);
				var bytes = Api.RetrieveColumn(session.JetSesid, table.JetTableid, pair.Columnid);
				switch (pair.Coltyp)
				{
					case JET_coltyp.Nil:
						break;
					case JET_coltyp.Bit:
						Console.WriteLine(BitConverter.ToBoolean(bytes,0));
						break;
					case JET_coltyp.UnsignedByte:
						Console.WriteLine(bytes[0]);
						break;
					case JET_coltyp.Short:
						Console.WriteLine(BitConverter.ToInt16(bytes, 0));
						break;
					case JET_coltyp.Long:
						Console.WriteLine(BitConverter.ToInt32(bytes, 0));
						break;
					case (JET_coltyp)15:
						Console.WriteLine(BitConverter.ToInt64(bytes, 0));
						break;
					case (JET_coltyp)16:
						Console.WriteLine(new Guid(bytes));
						break;
					case JET_coltyp.LongBinary:
					case JET_coltyp.Binary:
						Console.WriteLine("binary");
						break;
					case JET_coltyp.Text:
					case JET_coltyp.LongText:
						Console.WriteLine(Encoding.UTF8.GetString(bytes));
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
}