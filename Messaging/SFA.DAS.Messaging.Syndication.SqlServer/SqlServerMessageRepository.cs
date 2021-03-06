﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SFA.DAS.Messaging.Syndication.SqlServer
{
    public class SqlServerMessageRepository : IMessageRepository
    {
        private readonly string _connectionString;
        private readonly string _storeStoredProcedureName;
        private readonly string _retreiveStoredProcedureName;
        private readonly int _pageSize;

        public SqlServerMessageRepository(string connectionString, string storeStoredProcedureName, string retreiveStoredProcedureName, int pageSize)
        {
            _connectionString = connectionString;
            _storeStoredProcedureName = storeStoredProcedureName;
            _retreiveStoredProcedureName = retreiveStoredProcedureName;
            _pageSize = pageSize;
        }

        public async Task StoreAsync(object message)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                try
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = _storeStoredProcedureName;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("MessageBody", JsonConvert.SerializeObject(message)));

                        await command.ExecuteNonQueryAsync();
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task<SyndicationPage<T>> RetreivePageAsync<T>(int page)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                try
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = _retreiveStoredProcedureName;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        var numMessagesParam = new SqlParameter("TotalNumberOfMessages", System.Data.SqlDbType.BigInt)
                        {
                            Direction = System.Data.ParameterDirection.Output
                        };
                        command.Parameters.Add(new SqlParameter("PageNumber", page));
                        command.Parameters.Add(new SqlParameter("PageSize", _pageSize));
                        command.Parameters.Add(numMessagesParam);

                        var messages = new List<T>();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                messages.Add(JsonConvert.DeserializeObject<T>((string)reader["MessageBody"]));
                            }
                        }

                        var totalNumberOfMessages = Convert.ToInt32(numMessagesParam.Value);
                        var totalNumberOfPages = (int) Math.Ceiling(totalNumberOfMessages/(float) _pageSize);

                        return new SyndicationPage<T>
                        {
                            Messages = messages,
                            PageNumber = page,
                            TotalNumberOfMessages = totalNumberOfMessages,
                            TotalNumberOfPages = totalNumberOfPages
                        };
                    }
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
