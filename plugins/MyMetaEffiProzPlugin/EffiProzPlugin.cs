#if !IGNORE_EFFIPROZ
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using MyMeta;

using System.Data.EffiProz;
using EffiProz.Core;

namespace MyMeta.Plugins
{
    public class EffiProzPlugin : IMyMetaPlugin, IDisposable
    {
        //CREATE MEMORY TABLE PUBLIC.TEST_DATATYPES(FIELD_INTIDENTITY INTEGER GENERATED BY DEFAULT AS IDENTITY(START WITH 33) NOT NULL,FIELD_INT INTEGER,FIELD_TINYINT TINYINT,FIELD_BOOLEAN BOOLEAN,FIELD_SMALLINT SMALLINT,FIELD_BIGINT BIGINT,FIELD_DOUBLE DOUBLE,FIELD_DATE DATE,FIELD_UNIQUEIDENTIFIER UNIQUEIDENTIFIER,FIELD_VARCHAR VARCHAR(50),FIELD_VARCHAR2 VARCHAR(50),FIELD_CHAR CHAR(1),FIELD_BINARY BINARY(50),FIELD_VARBINARY VARBINARY(50),FIELD_CLOB CLOB(4),FIELD_BLOB BLOB(4),FIELD_NUMBER DECIMAL(10,4),FIELD_DECIMAL DECIMAL(6,2),FIELD_TIMESTAMP TIMESTAMP(3),FIELD_TIMESTAMPWITHTZ TIMESTAMP(3) WITH TIME ZONE,FIELD_INTERVALYEAR INTERVAL YEAR(2) TO MONTH,FIELD_INTERVALDAY INTERVAL DAY(2) TO SECOND(3),CONSTRAINT PK_TEST_DATATYPES PRIMARY KEY(FIELD_INTIDENTITY))
        private const string PROVIDER_KEY = @"EFFIPROZ";
        private const string PROVIDER_NAME = @"EffiProz";
        private const string AUTHOR_INFO = @"EffiProz MyMeta plugin written by MyGeneration Software.";
        private const string AUTHOR_URI = @"http://www.mygenerationsoftware.com/";
        private const string SAMPLE_CONNECTION = @"Connection Type=File; Auto Shutdown=false; ReadOnly=true; Initial Catalog=App_Data/Efz/Northwind; User=sa; Password=;";

        private string lastConnectionString = null;
        private EfzConnection currentConnection;
        private IMyMetaPluginContext context;

        public void Initialize(IMyMetaPluginContext context)
        {
            this.context = context;
            CloseInternalConnection();
        }

        public string ProviderName
        {
            get { return PROVIDER_NAME; }
        }

        public string ProviderUniqueKey
        {
            get { return PROVIDER_KEY; }
        }

        public string ProviderAuthorInfo
        {
            get { return AUTHOR_INFO; }
        }

        public Uri ProviderAuthorUri
        {
            get { return new Uri(AUTHOR_URI); }
        }

        public bool StripTrailingNulls
        {
            get { return false; }
        }

        public bool RequiredDatabaseName
        {
            get { return false; }
        }

        public string SampleConnectionString
        {
            get { return SAMPLE_CONNECTION; }
        }

        private EfzConnection InternalConnection
        {
            get
            {
                if (!context.ConnectionString.Equals(this.lastConnectionString) && (currentConnection != null))
                {
                    CloseInternalConnection();
                }

                if (currentConnection == null || currentConnection.State == ConnectionState.Broken || currentConnection.State == ConnectionState.Closed)
                {
                    string connString = this.context.ConnectionString;
                    lastConnectionString = connString;
 
                    int roIdx = connString.IndexOf("readonly", StringComparison.CurrentCultureIgnoreCase);
                    if (roIdx > -1)
                    {
                        int scIdx = connString.IndexOf(";", roIdx);
                        if (scIdx > roIdx) {
                            connString = connString.Substring(0, roIdx) + connString.Substring(scIdx + 1);
                        }
                        else {
                            connString = connString.Substring(0, roIdx);
                        }
                    }

                    if (!connString.Trim().EndsWith(";")) connString += ";";
                    connString += " ReadOnly=true;";

                    currentConnection = new EfzConnection(connString);
                    currentConnection.Open();
                    currentConnection.IsolationLevel = IsolationLevel.ReadUncommitted;
                }

                return currentConnection;
            }
        }

        private void CloseInternalConnection()
        {
            if (currentConnection != null)
            {
                if (currentConnection.State == ConnectionState.Open ||
                    currentConnection.State == ConnectionState.Fetching ||
                    currentConnection.State == ConnectionState.Executing ||
                    currentConnection.State == ConnectionState.Connecting)
                {
                    currentConnection.Close();
                }
                currentConnection = null;
            }
        }

        public IDbConnection NewConnection
        {
            get
            {
                if (IsIntialized)
                {
                    EfzConnection conn = new EfzConnection(this.context.ConnectionString);
                    return conn;
                }
                else
                    return null;
            }
        }

        public string DefaultDatabase
        {
            get
            {
                return this.GetDatabaseName();
            }
        }

        public DataTable Databases
        {
            get
            {
                DataTable metaData = new DataTable();
                try
                {
                    metaData = context.CreateDatabasesDataTable();

                    EfzConnection conn = InternalConnection;

                    EfzCommand cmd = new EfzCommand();
                    cmd.CommandText = "SELECT DISTINCT TABLE_CAT, TABLE_SCHEM FROM INFORMATION_SCHEMA.SYSTEM_TABLES WHERE TABLE_TYPE='TABLE'";
                    cmd.Connection = conn;

                    using (EfzDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            DataRow row = metaData.NewRow();
                            metaData.Rows.Add(row);

                            row["CATALOG_NAME"] = r["TABLE_CAT"];
                            row["SCHEMA_NAME"] = r["TABLE_SCHEM"];
                            row["DESCRIPTION"] = conn.DataSource;
                        }
                    }

                }
                finally { }

                return metaData;
            }
        }

        public DataTable GetTables(string database)
        {
            DataTable metaData = new DataTable();
            /*SYSTEM_TABLES*/
            try
            {
                metaData = context.CreateTablesDataTable();

                EfzConnection conn = InternalConnection;

                EfzCommand cmd = new EfzCommand();
                cmd.CommandText = "SELECT * FROM INFORMATION_SCHEMA.SYSTEM_TABLES WHERE TABLE_TYPE='TABLE' AND TABLE_CAT='" + database + "'";
                cmd.Connection = conn;

                using (EfzDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        DataRow row = metaData.NewRow();
                        metaData.Rows.Add(row);

                        row["TABLE_CATALOG"] = r["TABLE_CAT"];
                        row["TABLE_SCHEMA"] = r["TABLE_SCHEM"];
                        row["TABLE_NAME"] = r["TABLE_NAME"];
                        row["TABLE_TYPE"] = (r["TABLE_TYPE"].ToString().IndexOf("SYSTEM ") > -1) ? "SYSTEM TABLE" : "TABLE";
                        row["DESCRIPTION"] = r["REMARKS"] + " : " + r["EFFIPROZ_TYPE"] + (r["COMMIT_ACTION"] == DBNull.Value ? string.Empty : (" " + r["COMMIT_ACTION"]));
                    }
                }

            }
            finally { }

            return metaData;
        }

        public DataTable GetViews(string database)
        {
            DataTable metaData = new DataTable();

            try
            {
                metaData = context.CreateViewsDataTable();

                EfzConnection conn = InternalConnection;

                EfzCommand cmd = new EfzCommand();
                cmd.CommandText =
@"SELECT t.TABLE_CAT, t.TABLE_SCHEM, v.TABLE_NAME, v.VIEW_DEFINITION, v.CHECK_OPTION, t.REMARKS, t.EFFIPROZ_TYPE, t.TABLE_TYPE, v.IS_UPDATABLE
FROM INFORMATION_SCHEMA.VIEWS v 
left join INFORMATION_SCHEMA.SYSTEM_TABLES t on t.TABLE_NAME = v.TABLE_NAME";
                cmd.Connection = conn;

                using (EfzDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        DataRow row = metaData.NewRow();
                        metaData.Rows.Add(row);
                        row["TABLE_CATALOG"] = r["TABLE_CAT"];
                        row["TABLE_SCHEMA"] = r["TABLE_SCHEM"];
                        row["TABLE_NAME"] = r["TABLE_NAME"];
                        row["TABLE_TYPE"] = (r["TABLE_TYPE"].ToString().IndexOf("SYSTEM ") > -1) ? "SYSTEM VIEW" : "VIEW";
                        row["DESCRIPTION"] = r["REMARKS"] + " : " + r["EFFIPROZ_TYPE"] + (r["CHECK_OPTION"] == DBNull.Value ? string.Empty : (" " + r["CHECK_OPTION"]));
                        row["IS_UPDATABLE"] = r["IS_UPDATABLE"].ToString().Equals("YES", StringComparison.CurrentCultureIgnoreCase);
                        //row["INSERTABLE_INTO"] = r["INSERTABLE_INTO"]; // NOT SUPPORTED YET
                        row["VIEW_TEXT"] = r["VIEW_DEFINITION"];

                    }
                }
            }
            finally { }

            return metaData;
        }

        public DataTable GetProcedures(string database)
        {
            DataTable metaData = new DataTable();

            try
            {
                metaData = context.CreateProceduresDataTable();

                EfzConnection conn = InternalConnection;

                EfzCommand cmd = new EfzCommand();
                cmd.CommandText =
@"SELECT p.*
FROM INFORMATION_SCHEMA.SYSTEM_PROCEDURES p
WHERE PROCEDURE_CAT = '" + database + "'";
                cmd.Connection = conn;

                using (EfzDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        DataRow row = metaData.NewRow();
                        metaData.Rows.Add(row);
                        row["PROCEDURE_CATALOG"] = r["PROCEDURE_CAT"];
                        row["PROCEDURE_SCHEMA"] = r["PROCEDURE_SCHEM"];
                        row["PROCEDURE_NAME"] = r["PROCEDURE_NAME"];
                        row["PROCEDURE_TYPE"] = r["PROCEDURE_TYPE"];
                        row["DESCRIPTION"] = r["REMARKS"];
                    }
                }
            }
            finally { }

            return metaData;
        }

        public DataTable GetDomains(string database)
        {
            return new DataTable();
        }

        public DataTable GetProcedureParameters(string database, string procedure)
        {
            DataTable metaData = new DataTable();

            try
            {
                metaData = context.CreateParametersDataTable();

                EfzConnection conn = InternalConnection;

                EfzCommand cmd = new EfzCommand();
                cmd.CommandText = 
@"SELECT * 
FROM INFORMATION_SCHEMA.SYSTEM_PROCEDURECOLUMNS
WHERE PROCEDURE_NAME='" + procedure + "' and PROCEDURE_CAT='" + database + "'";
                cmd.Connection = conn;

                using (EfzDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        DataRow row = metaData.NewRow();
                        metaData.Rows.Add(row);

                        if (r["IS_NULLABLE"] != DBNull.Value)
                        {
                            row["IS_NULLABLE"] = r["NULLABLE"];
                        }

                        int type = Convert.ToInt32(r["DATA_TYPE"]); // dbType enum code
                        string typeName = (string)r["TYPE_NAME"]; // dbType enum code
                        int charMax = 0;
                        int charOctetMax = 0;
                        int precision = 0;
                        int scale = 0;

                        if (r["CHAR_OCTET_LENGTH"] != DBNull.Value)
                        {
                            charOctetMax = Convert.ToInt32(r["CHAR_OCTET_LENGTH"]);
                        }

                        if (r["LENGTH"] != DBNull.Value)
                        {
                            charMax = Convert.ToInt32(r["LENGTH"]);
                        }

                        if (r["PRECISION"] != DBNull.Value)
                        {
                            precision = Convert.ToInt32(r["PRECISION"]);
                        }

                        if (r["SCALE"] != DBNull.Value)
                        {
                            scale = Convert.ToInt32(r["SCALE"]);
                        }

                        row["DATA_TYPE"] = type;
                        row["TYPE_NAME"] = typeName;
                        //row["TYPE_NAME_COMPLETE"] = this.GetDataTypeNameComplete(typeName, charMax, precision, scale);

                        row["CHARACTER_MAXIMUM_LENGTH"] = charMax;
                        row["CHARACTER_OCTET_LENGTH"] = charOctetMax;
                        row["NUMERIC_PRECISION"] = precision;
                        row["NUMERIC_SCALE"] = scale;


                        row["PROCEDURE_CATALOG"] = r["PROCEDURE_CAT"];
                        row["PROCEDURE_SCHEMA"] = r["PROCEDURE_SCHEM"];
                        row["PROCEDURE_NAME"] = r["PROCEDURE_NAME"];
                        row["PARAMETER_NAME"] = r["COLUMN_NAME"];
                        row["ORDINAL_POSITION"] = r["ORDINAL_POSITION"];
                        row["PARAMETER_TYPE"] = r["COLUMN_TYPE"];
                        row["PARAMETER_HASDEFAULT"] = r["COLUMN_DEF"] != DBNull.Value && r["COLUMN_DEF"] != string.Empty;
                        row["PARAMETER_DEFAULT"] = r["COLUMN_DEF"];
                        //row["IS_NULLABLE"] = r["NULLABLE"];
                        //row["DATA_TYPE"] = r["DATA_TYPE"];
                        //row["CHARACTER_MAXIMUM_LENGTH"] = r["LENGTH"];
                        //row["CHARACTER_OCTET_LENGTH"] = r["CHAR_OCTET_LENGTH"];
                        row["NUMERIC_PRECISION"] = r["PRECISION"];
                        row["NUMERIC_SCALE"] = r["SCALE"];
                        row["DESCRIPTION"] = r["REMARKS"];
                        //row["TYPE_NAME"] = r["TYPE_NAME"];
                        //row["LOCAL_TYPE_NAME"] = r[""];
                    }
                }
            }
            finally { }

            return metaData;
        }

        public DataTable GetProcedureResultColumns(string database, string procedure)
        {
            return new DataTable();
        }

        public DataTable GetViewColumns(string database, string view)
        {
            DataTable metaData = new DataTable();

            try
            {
                metaData = context.CreateColumnsDataTable();

                EfzConnection conn = InternalConnection;

                EfzCommand cmd = new EfzCommand();
                cmd.CommandText = "SELECT * FROM INFORMATION_SCHEMA.SYSTEM_COLUMNS WHERE TABLE_NAME='" + view + "' and TABLE_CAT='" + database + "'";
                cmd.Connection = conn;

                using (EfzDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        DataRow row = metaData.NewRow();
                        metaData.Rows.Add(row);

                        row["TABLE_CATALOG"] = r["TABLE_CAT"];
                        row["TABLE_SCHEMA"] = r["TABLE_SCHEM"];
                        row["TABLE_NAME"] = r["TABLE_NAME"];
                        row["COLUMN_NAME"] = r["COLUMN_NAME"];
                        row["ORDINAL_POSITION"] = r["ORDINAL_POSITION"];
                        row["DESCRIPTION"] = r["REMARKS"];
                        row["COLUMN_HASDEFAULT"] = false;

                        if (r["IS_NULLABLE"] != DBNull.Value)
                        {
                            row["IS_NULLABLE"] = r["IS_NULLABLE"];
                        }

                        if (r["COLUMN_DEF"] != DBNull.Value)
                        {
                            row["COLUMN_HASDEFAULT"] = true;
                            row["COLUMN_DEFAULT"] = r["COLUMN_DEF"];
                        }

                        if (r["IS_GENERATED"] != DBNull.Value && r["IDENTITY_INCREMENT"] != DBNull.Value)
                        {
                            row["IS_AUTO_KEY"] = true;
                            row["AUTO_KEY_SEED"] = Convert.ToInt32(r["IDENTITY_START"]);
                            row["AUTO_KEY_INCREMENT"] = Convert.ToInt32(r["IDENTITY_INCREMENT"]);
                        }

                        int type = Convert.ToInt32(r["DATA_TYPE"]); // dbType enum code
                        string typeName = (string)r["TYPE_NAME"]; // dbType enum code
                        int charMax = 0;
                        int precision = 0;
                        int scale = 0;

                        if (r["COLUMN_SIZE"] != DBNull.Value)
                        {
                            charMax = Convert.ToInt32(r["COLUMN_SIZE"]);
                        }

                        if (r["COLUMN_SIZE"] != DBNull.Value)
                        {
                            precision = Convert.ToInt32(r["COLUMN_SIZE"]);
                        }

                        if (r["DECIMAL_DIGITS"] != DBNull.Value)
                        {
                            scale = Convert.ToInt32(r["DECIMAL_DIGITS"]);
                        }

                        row["DATA_TYPE"] = type;
                        row["TYPE_NAME"] = typeName;
                        row["TYPE_NAME_COMPLETE"] = this.GetDataTypeNameComplete(typeName, charMax, precision, scale);

                        row["NUMERIC_PRECISION"] = precision;
                        row["NUMERIC_SCALE"] = scale;

                        row["CHARACTER_MAXIMUM_LENGTH"] = charMax;

                        //TODO: we will have to find the best way to implement this later?
                        //row["IS_COMPUTED"] = (type == "timestamp") ? true : false;
                    }
                }
            }
            finally { }

            return metaData;
        }

        public DataTable GetTableColumns(string database, string table)
        {
            DataTable metaData = new DataTable();

            try
            {
                metaData = context.CreateColumnsDataTable();

                EfzConnection conn = InternalConnection;

                EfzCommand cmd = new EfzCommand();
                cmd.CommandText = "SELECT * FROM INFORMATION_SCHEMA.SYSTEM_COLUMNS WHERE TABLE_NAME='" + table + "' AND TABLE_CAT='" + database + "'";
                cmd.Connection = conn;

                using (EfzDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        DataRow row = metaData.NewRow();
                        metaData.Rows.Add(row);

                        row["TABLE_CATALOG"] = r["TABLE_CAT"];
                        row["TABLE_SCHEMA"] = r["TABLE_SCHEM"];
                        row["TABLE_NAME"] = r["TABLE_NAME"];
                        row["COLUMN_NAME"] = r["COLUMN_NAME"];
                        row["ORDINAL_POSITION"] = r["ORDINAL_POSITION"];
                        row["DESCRIPTION"] = r["REMARKS"];
                        row["COLUMN_HASDEFAULT"] = false;

                        if (r["IS_NULLABLE"] != DBNull.Value)
                        {
                            row["IS_NULLABLE"] = r["IS_NULLABLE"];
                        }

                        if (r["COLUMN_DEF"] != DBNull.Value)
                        {
                            row["COLUMN_HASDEFAULT"] = true;
                            row["COLUMN_DEFAULT"] = r["COLUMN_DEF"];
                        }

                        if (r["IS_GENERATED"] != DBNull.Value && r["IDENTITY_INCREMENT"] != DBNull.Value)
                        {
                            row["IS_AUTO_KEY"] = true;
                            row["AUTO_KEY_SEED"] = Convert.ToInt32(r["IDENTITY_START"]);
                            row["AUTO_KEY_INCREMENT"] = Convert.ToInt32(r["IDENTITY_INCREMENT"]);
                        }

                        int type = Convert.ToInt32(r["DATA_TYPE"]); // dbType enum code
                        string typeName = (string)r["TYPE_NAME"]; // dbType enum code
                        int charMax = 0;
                        int precision = 0;
                        int scale = 0;

                        if (r["COLUMN_SIZE"] != DBNull.Value)
                        {
                            charMax = Convert.ToInt32(r["COLUMN_SIZE"]);
                        }

                        if (r["COLUMN_SIZE"] != DBNull.Value)
                        {
                            precision = Convert.ToInt32(r["COLUMN_SIZE"]);
                        }

                        if (r["DECIMAL_DIGITS"] != DBNull.Value)
                        {
                            scale = Convert.ToInt32(r["DECIMAL_DIGITS"]);
                        }

                        row["DATA_TYPE"] = type;
                        row["TYPE_NAME"] = typeName;
                        row["TYPE_NAME_COMPLETE"] = this.GetDataTypeNameComplete(typeName, charMax, precision, scale);

                        row["NUMERIC_PRECISION"] = precision;
                        row["NUMERIC_SCALE"] = scale;

                        row["CHARACTER_MAXIMUM_LENGTH"] = charMax;

                        //TODO: we will have to find the best way to implement this later?
                        //row["IS_COMPUTED"] = (type == "timestamp") ? true : false;
                    }
                }
            }
            finally { }

            return metaData;
        }

        public List<string> GetPrimaryKeyColumns(string database, string table)
        {
            List<string> primaryKeys = new List<string>();

            try
            {

                    EfzConnection conn = InternalConnection;

                    EfzCommand cmd = new EfzCommand();
                    cmd.CommandText = "SELECT * FROM INFORMATION_SCHEMA.SYSTEM_PRIMARYKEYS WHERE TABLE_NAME='" + table + "' AND TABLE_CAT='" + database + "'";
                    cmd.Connection = conn;

                    using (EfzDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            primaryKeys.Add(r["COLUMN_NAME"].ToString());
                        }
                    }
            }
            finally { }

            return primaryKeys;
        }

        public List<string> GetViewSubViews(string database, string view)
        {
            return new List<string>();
        }

        public List<string> GetViewSubTables(string database, string view)
        {
            return new List<string>();
        }

        public DataTable GetTableIndexes(string database, string table)
        {
            DataTable metaData = new DataTable();

            try
            {
                metaData = context.CreateIndexesDataTable();

                EfzConnection conn = InternalConnection;

                EfzCommand cmd = new EfzCommand();
                cmd.CommandText = "SELECT * FROM INFORMATION_SCHEMA.SYSTEM_INDEXINFO WHERE TABLE_NAME='" +
                    table + "' AND TABLE_CAT='" + database + "'";
                cmd.Connection = conn;

                DataTable dt = new DataTable();
                EfzDataAdapter adapter = new EfzDataAdapter();
                adapter.SelectCommand = cmd;
                adapter.Fill(dt);

                foreach (DataRow r in dt.Rows)
                {
                    DataRow row = metaData.NewRow();
                    metaData.Rows.Add(row);

                    row["TABLE_CATALOG"] = r["TABLE_CAT"];
                    row["TABLE_SCHEMA"] = r["TABLE_SCHEM"];
                    row["TABLE_NAME"] = r["TABLE_NAME"];
                    row["INDEX_NAME"] = r["INDEX_NAME"];
                    row["UNIQUE"] = r["UNIQUE_INDEX"];
                    row["PRIMARY_KEY"] = r["PRIMARY_INDEX"];
                    row["CARDINALITY"] = r["ROW_CARDINALITY"] == DBNull.Value ? (object)DBNull.Value : (object)Convert.ToDecimal(r["ROW_CARDINALITY"]);
                    row["COLUMN_NAME"] = r["COLUMN_NAME"];
                    row["FILTER_CONDITION"] = r["FILTER_CONDITION"];
                    row["TYPE"] = r["TYPE"];
                    row["PAGES"] = r["PAGES"];
                    row["ORDINAL_POSITION"] = r["ORDINAL_POSITION"];                
                }
            }
            finally { }

            return metaData;
        }

        public DataTable GetForeignKeys(string database, string table)
        {
            DataTable metaData = new DataTable();

            try
            {
                metaData = context.CreateForeignKeysDataTable();

                EfzConnection conn = InternalConnection;

                EfzCommand cmd = new EfzCommand();
                cmd.CommandText = "SELECT * FROM INFORMATION_SCHEMA.SYSTEM_CROSSREFERENCE WHERE (PKTABLE_NAME='" +
                    table + "' AND PKTABLE_CAT='" + database + "') OR (FKTABLE_NAME='" +
                    table + "' AND FKTABLE_CAT='" + database + "')";
                cmd.Connection = conn;

                DataTable dt = new DataTable();
                EfzDataAdapter adapter = new EfzDataAdapter();
                adapter.SelectCommand = cmd;
                adapter.Fill(dt);

                foreach (DataRow r in dt.Rows)
                {
                    DataRow row = metaData.NewRow();
                    metaData.Rows.Add(row);

                    // The main Information ...
                    row["PK_TABLE_CATALOG"] = r["PKTABLE_CAT"];
                    row["PK_TABLE_SCHEMA"] = r["PKTABLE_SCHEM"];
                    row["PK_TABLE_NAME"] = r["PKTABLE_NAME"];
                    row["FK_TABLE_CATALOG"] = r["FKTABLE_CAT"];
                    row["FK_TABLE_SCHEMA"] = r["FKTABLE_SCHEM"];
                    row["FK_TABLE_NAME"] = r["FKTABLE_NAME"];
                    row["ORDINAL"] = r["KEY_SEQ"];
                    row["FK_NAME"] = r["FK_NAME"];
                    row["PK_NAME"] = r["PK_NAME"];
                    row["UPDATE_RULE"] = r["UPDATE_RULE"];
                    row["DELETE_RULE"] = r["DELETE_RULE"];
                    row["DEFERRABILITY"] = r["DEFERRABILITY"];
                    row["PK_COLUMN_NAME"] = r["PKCOLUMN_NAME"];
                    row["FK_COLUMN_NAME"] = r["FKCOLUMN_NAME"];
                }
            }
            finally { }

            return metaData;
        }

        public object GetDatabaseSpecificMetaData(object myMetaObject, string key)
        {
            return null;
        }
        
        private bool IsIntialized
        {
            get
            {
                return (context != null);
            }
        }

        public string GetDatabaseName()
        {
            string db = "PUBLIC";
            try
            {
                EfzConnection conn = InternalConnection;
                EfzCommand cmd = new EfzCommand();
                cmd.CommandText = "SELECT DISTINCT SCHEMA FROM INFORMATION_SCHEMA.SYSTEM_SESSIONS";
                cmd.Connection = conn;
                using (EfzDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        db = reader["SCHEMA"].ToString();
                    }
                    reader.Close();
                }
            }
            finally
            {
                //EfzConnection.ClearAllPools();
            }

            return db;
        }

        public string GetFullDatabaseName()
        {
            string source = null;
            using (EfzConnection conn = new EfzConnection(this.context.ConnectionString))
            {
                source = conn.DataSource;
            }
            return source;
        }


        private string GetDataTypeNameComplete(string dataType, int charMax, int precision, int scale)
        {
            StringBuilder sb = new StringBuilder();

            switch (dataType.ToUpper())
            {
                case "BOOLEAN":
                case "TINYINT":
                case "SMALLINT":
                case "INT":
                case "INTEGER":
                case "BIGINT":
                case "DOUBLE":
                case "DATE":
                case "UNIQUEIDENTIFIER":
                    sb.Append(dataType);
                    break;
                case "CHAR":
                case "VARCHAR":
                case "VARCHAR2":
                case "BINARY":
                case "VARBINARY":
                case "CLOB":
                case "BLOB":
                    sb.Append(dataType).Append('(').Append(charMax).Append(')');
                    break;
                case "NUMBER":
                case "DECIMAL":
                    sb.Append(dataType).Append('(').Append(precision).Append(", ").Append(scale).Append(')');
                    break;
                case "TIMESTAMP":
                    sb.Append(dataType);
                    if (precision > 0)
                    {
                        sb.Append('(').Append(precision).Append(')');
                    }
                    break;
                case "TIMESTAMP WITH TIME ZONE":
                    sb.Append("TIMESTAMP");
                    if (precision > 0)
                    {
                        sb.Append('(').Append(precision).Append(')');
                    }
                    sb.Append(" WITH TIME ZONE");
                    break;
                case "INTERVAL YEAR TO MONTH":
                    sb.Append("INTERVAL YEAR");
                    if (precision > 0)
                    {
                        sb.Append('(').Append(precision).Append(')');
                    }
                    sb.Append(" TO MONTH");
                    break;
                case "INTERVAL DAY TO SECOND":
                    sb.Append("INTERVAL DAY");
                    if (precision > 0)
                    {
                        sb.Append('(').Append(precision).Append(')');
                    }
                    sb.Append(" TO SECOND");
                    if (scale > 0)
                    {
                        sb.Append('(').Append(scale).Append(')');
                    }
                    break;
                default:
                    System.Diagnostics.Debugger.Break();
                    break;
            }
            return sb.ToString();
        }


        public void Dispose()
        {
            this.CloseInternalConnection();
        }
    }
}
#endif