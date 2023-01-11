using Azure;
using Azure.Data.Tables;
using DataServices.Models.SystemData;
namespace DataServices.Services;


public interface IDataTableBaseOperation<TEntity> where TEntity : class, ITableEntity, new()
{
    Task<AsyncPageable<TEntity>> QueryAllAsync();
    Task<AsyncPageable<TEntity>> QueryWithFilterAsync(string filters);
    Task<TEntity> GetAsync(string rowKey);

    Task<TEntity> AddOrUpdateAsync(TEntity entity);
    Task<TEntity> InsertAsync(TEntity entity);
    Task<Response> UpdateAsync(TEntity entity);
    Task<Response> DeleteAsync(string rowKey);
}

internal class DataTableBaseOperation<TEntity> : IDataTableBaseOperation<TEntity> where TEntity : class, ITableEntity, new()
{
    private static TableServiceClient tableServiceClient;
    private static TableClient? tableClient;
    private readonly string tableName;
    private static readonly string mainPartionKey = "MAIN";
    private static readonly string tableInformationRowKey = "SYSTEM-TableInformation";

    public DataTableBaseOperation(string connectionString, string tableName)
    {
        this.tableName = tableName;
        tableServiceClient = new(connectionString);
    }

    private async Task InitializeTableClient()
    {
        if (tableClient is null)
        {
            // Check if table exists
            bool exists = false;
            await foreach (var tbl in tableServiceClient.QueryAsync(t => t.Name == tableName))
            {
                exists = true;
            }

            // Get the table client. Create the table if it doesn't exists
            tableClient = tableServiceClient.GetTableClient(tableName);
            await tableClient.CreateIfNotExistsAsync();

            if (!exists)
            {
                // Store "metadata" for no of lines, etc
                TableEntity entity = new(mainPartionKey, tableInformationRowKey)
                    {
                        { "LineCount", 0 },
                        { "LastRowKey", "0000000000" }
                    };
                await tableClient.AddEntityAsync(entity);
            }

        }
    }

    public async Task<AsyncPageable<TEntity>> QueryAllAsync()
    {
        await InitializeTableClient();
        AsyncPageable<TEntity> queryResultsFilter;

        queryResultsFilter = tableClient.QueryAsync<TEntity>(filter: $"(PartitionKey eq '{mainPartionKey}') and (RowKey ne '{tableInformationRowKey}')");
        return queryResultsFilter;
    }

    public async Task<AsyncPageable<TEntity>> QueryWithFilterAsync(string filters)
    {
        AsyncPageable<TEntity> queryResults;
        await InitializeTableClient();
        if (string.IsNullOrWhiteSpace(filters))
        {
            queryResults = tableClient.QueryAsync<TEntity>(filter: $"(RowKey ne '{tableInformationRowKey}')");
        }
        else
        {
            queryResults = tableClient.QueryAsync<TEntity>(filter: $"{filters} and (RowKey ne '{tableInformationRowKey}')");
        }

        return queryResults;
    }

    public async Task<TEntity> GetAsync(string rowKey)
    {
        await InitializeTableClient();
        var queryResultsFilter = await tableClient.GetEntityAsync<TEntity>(
            partitionKey: mainPartionKey,
            rowKey: rowKey
        );

        return queryResultsFilter;
    }

    public async Task<TEntity> AddOrUpdateAsync(TEntity entity)
    {
        if (string.IsNullOrEmpty(entity.PartitionKey))
            entity.PartitionKey = mainPartionKey;
        await InitializeTableClient();
        await tableClient.UpsertEntityAsync(entity);
        return entity;
    }

    public async Task<TEntity> InsertAsync(TEntity entity)
    {
        await InitializeTableClient();

        // TODO: Implement Batch-insert to create a transaction in case of any error
        TableInformation informationEntity = await tableClient.GetEntityAsync<TableInformation>(rowKey: tableInformationRowKey, partitionKey: mainPartionKey);
        if (informationEntity is not null)
        {
            entity.PartitionKey = mainPartionKey;
            entity.RowKey = IncreaseStringNumber(informationEntity.LastRowKey, 1);
            informationEntity.LineCount += 1;
            informationEntity.LastRowKey = entity.RowKey;
            await tableClient.UpsertEntityAsync(informationEntity);
        }

        if (string.IsNullOrWhiteSpace(entity.PartitionKey))
            entity.PartitionKey = mainPartionKey;
        await tableClient.AddEntityAsync(entity);
        return entity;
    }
    public async Task<Response> UpdateAsync(TEntity entity)
    {
        await InitializeTableClient();
        var entityReturn = await tableClient.UpdateEntityAsync(entity, entity.ETag);
        return entityReturn;
    }

    public async Task<Response> DeleteAsync(string rowKey)
    {
        await InitializeTableClient();
        TableInformation informationEntity = await tableClient.GetEntityAsync<TableInformation>(rowKey: tableInformationRowKey, partitionKey: mainPartionKey);
        if (informationEntity is not null)
        {
            informationEntity.LineCount -= 1;
            if (informationEntity.LineCount < 0)
            {
                informationEntity.LineCount = 0;
            }
            await tableClient.UpsertEntityAsync(informationEntity);
        }

        var entity = await tableClient.DeleteEntityAsync(rowKey: rowKey, partitionKey: mainPartionKey);
        return entity;
    }

    private string IncreaseStringNumber(string s, int increaseBy = 1)
    {
        string numberString = string.Empty;
        string restOfString = string.Empty;
        int val;

        for (int i = 0; i < s.Length; i++)
        {
            if (char.IsDigit(s[i]))
                numberString += s[i];
            else
                restOfString += s[i];
        }

        if (numberString.Length == 0)
            return s;

        val = int.Parse(numberString) + increaseBy;

        return restOfString + val.ToString().PadLeft((s.Length - restOfString.Length), '0');
    }
}
