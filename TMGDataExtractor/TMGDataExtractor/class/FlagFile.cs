﻿using System;
using System.Data.OleDb;
using System.Data.Common;
using System.Data.SQLite;
using System.Configuration;

namespace TMG.DataExtractor
{
	public class FlagFile : VFPDataAccess
	{
		public FlagFile()
		{
			OleDbDataReader oledbReader;
			oledbReader = base.GetOleDbDataReader("*_c.dbf");			

			using (var conn = new SQLiteConnection(ConfigurationManager.ConnectionStrings["TMG.DataExtractor.Properties.Settings.tmgConnectionString"].ToString()))
			{
				conn.Open();

				using (var cmd = new SQLiteCommand(conn))
				{
					using (var transaction = conn.BeginTransaction())
					{
						cmd.CommandText = "DELETE FROM Flag;";
						cmd.ExecuteNonQuery();

						foreach (DbDataRecord row in oledbReader)
						{
							string sql = "INSERT INTO Flag (FLAGLABEL,FLAGFIELD,FLAGVALUE,SEQUENCE,DESCRIPT,ACTIVE,FLAGID,PROPERTY,DSID,TT) ";
							sql += string.Format("VALUES ('{0}','{1}','{2}',{3},'{4}','{5}',{6},'{7}',{8},'{9}');",
								row["FLAGLABEL"].ToString().Replace("'", "`")	,
								row["FLAGFIELD"].ToString().Replace("'", "`")	,
								row["FLAGVALUE"].ToString().Replace("'", "`")	,
								(int)row["SEQUENCE"]													,
								row["DESCRIPT"].ToString().Replace("'", "`")	,
								(bool)row["ACTIVE"]														,
								(int)row["FLAGID"]														,
								row["PROPERTY"].ToString().Replace("'", "`")	,
								(int)row["DSID"]															,
								row["TT"].ToString()
							);			

							cmd.CommandText = sql;
							cmd.ExecuteNonQuery();
							Tracer("Flags Added: {0} {1}%");
						}
						transaction.Commit();
					}
				}
				conn.Close();
			}
		}
	}
}


