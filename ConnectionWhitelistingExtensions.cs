using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Connection.Identifier.Quoting;

namespace Dapper.Contrib.Extensions.Tablename.ConnectionWhitelisting
{
    public static class ConnectionWhitelistingExtensions
    {
        //private static readonly IDbConnection connection;
        public static async Task WhitelistDatabaseTablesAsync(this IDbConnection connection)
        {
            const string sql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES";
            var tableNamesIEnumerable = await connection.QueryAsync<string>(sql);
            var tableNames = tableNamesIEnumerable.AsList().ToArray();
            for (int i = 0; i < tableNames.Length; i++)
            {
                tableNames[i] = tableNames[i].QuoteIdentifier(connection);
            }
            var whitelist = new HashSet<string>(tableNames);


            SqlMapperExtensions.TableNameMapper = (Type t) =>
            {
                var tableName = SqlMapperExtensions.TableNameMapper(t).QuoteIdentifier(connection);
                if (whitelist != null)
                {
                    return whitelist.Contains(tableName) ? tableName : throw new Exception($"The tablename {tableName} is not whitelisted!");
                }
                else
                {
                    return tableName;
                }
            };
        }
    }
}
