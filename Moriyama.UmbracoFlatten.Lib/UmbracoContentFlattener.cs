using log4net;
using Moriyama.UmbracoFlatten.Lib.Models;
using Moriyama.UmbracoFlatten.Lib.Sql;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Moriyama.UmbracoFlatten.Lib
{
    public class UmbracoContentFlattener
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _connectionString;

        public UmbracoContentFlattener(string connectionString)
        {
            _connectionString = connectionString;

        }

        public void Flatten()
        {

            var aliases = GetAllPropertyAliases();

            var sqlColums = new List<string>();

            foreach (var alias in aliases)
            {
                Logger.Info("Found alias ->" + alias);

                var values = GetAllPropertyValuesForAlias(alias);
                var types = SqlColumns(values);


                if (types.Count() > 1)
                    sqlColums.Add(alias + " nvarchar(max) null");
                else
                    sqlColums.Add(alias + " " + SqlTypeName(types.First()) + " null");

            }

            var sql = string.Join("," + Environment.NewLine, sqlColums);
            sql = "Create Table MoriyamaContent (id int not null, " + sql + ")";


            Logger.Info(sql);
            var contentIds = GetAllContentIds();

            using(var d = new Database(_connectionString))
            {
                d.Execute(@"IF OBJECT_ID('MoriyamaContent', 'U') IS NOT NULL DROP TABLE MoriyamaContent;");
                d.Execute(sql);

                foreach (var contentId in contentIds)
                {
                    Logger.Info("Got content with id : " + contentId);
                    var valuesForContent = GetAllValuesForContent(contentId);

                    var insertSql = BuildInsertStatement(contentId, valuesForContent);
                    d.Execute(insertSql);
                }
            }


        }

        private PetaPoco.Sql BuildInsertStatement(int id, PropertyValue[] propertyValues)
        {
            var a = PetaPoco.Sql.Builder.Append("insert into MoriyamaContent (id, ");

            for (int i = 0; i < propertyValues.Length; i++)
            {
                a.Append(propertyValues[i].Alias);
                if (i != propertyValues.Length - 1)
                {
                    a.Append(",");
                }
            }

            a.Append(") values (@0, ", id);


            for (int i = 0; i < propertyValues.Length; i++)
            {
                a.Append("@0", propertyValues[i].Value);

                if (i != propertyValues.Length - 1)
                {
                    a.Append(",");
                }
            }

            a.Append(")");
            return a;
        }

        private PropertyValue[] GetAllValuesForContent(int id)
        {
            IEnumerable<PropertyValue> values;
            using (var database = new Database(_connectionString))

                values = database.Fetch<PropertyValue>(Constants.GetAllValuesForContent, id);

            return values.ToArray();
        }

        private IEnumerable<int> GetAllContentIds()
        {
            IEnumerable<int> ids;
            using (var database = new Database(_connectionString))

                ids = database.Fetch<int>(Constants.GetAllContentIds);

            return ids;
        }

        private IEnumerable<Type> SqlColumns(IEnumerable<string> values)
        {
            var types = new List<Type>();
            foreach (var value in values)
            {
                var type = DetectType(value);

                if (type != null && !types.Contains(type.GetType()))
                    types.Add(type.GetType());

            }

            return types;
        }

        private string SqlTypeName(Type t)
        {
            switch (t.Name.ToLower())
            {
                case "int32":
                    return "int";
                case "datetime":
                    return "datetime";
                default:
                    return "nvarchar(max)";
            }
        }

        private object DetectType(string stringValue)
        {
            var expectedTypes = new List<Type> { typeof(DateTime), typeof(int), typeof(double), typeof(decimal), typeof(decimal), typeof(bool), typeof(string) };

            foreach (var type in expectedTypes)
            {
                TypeConverter converter = TypeDescriptor.GetConverter(type);
                if (converter.CanConvertFrom(typeof(string)))
                {
                    try
                    {
                        // You'll have to think about localization here
                        object newValue = converter.ConvertFromInvariantString(stringValue);
                        if (newValue != null)
                        {
                            return newValue;
                        }
                    }
                    catch
                    {
                        // Can't convert given string to this type
                        continue;
                    }
                }
            }

            return null;
        }

        private IEnumerable<string> GetAllPropertyAliases()
        {
            IEnumerable<string> aliases;
            using(var database = new Database(_connectionString))

                aliases = database.Fetch<string>(Constants.GetAllPropertyAliasesSql);

            return aliases;
        }

        private IEnumerable<string> GetAllPropertyValuesForAlias(string alias)
        {

            IEnumerable<string> values;
            using (var database = new Database(_connectionString))

                values = database.Fetch<string>(Constants.GetAllPropertyValuesForAlias, alias);

            return values;
        }

    }
}
