using DataServices.Models;

namespace DataServices.Services;

public class CustomerActivityService
{
    public readonly string tableName = "CustomerActivity";
    private readonly DataTableBaseOperation<CustomerActivity> tableOperation;

    public CustomerActivityService(string connectionString)
    {
        tableOperation = new(connectionString, tableName);
    }

    public async Task<List<TEntity>> GetAsync<TEntity>(string id) where TEntity : class
    {

        List<TEntity> list = new();
        if (string.IsNullOrEmpty(id))
        {
            var data = await tableOperation.QueryAllAsync();
            await foreach (var item in data)
            {
                item.Id = item.RowKey;
                list.Add(Helpers.Clone<CustomerActivity, TEntity>(item));
            }
        }
        else
        {
            var item = await tableOperation.GetAsync(id);
            item.Id = item.RowKey;
            list.Add(Helpers.Clone<CustomerActivity, TEntity>(item));
        }
        return list;
    }

    public async Task<TEntity> AddOrUpdateAsync<TEntity>(TEntity customerActivity) where TEntity : class
    {
        var dbcustomerActivity = Helpers.Clone<TEntity, CustomerActivity>(customerActivity);
        dbcustomerActivity.RowKey = dbcustomerActivity.Id;

        if (string.IsNullOrEmpty(dbcustomerActivity.RowKey))
        {
            dbcustomerActivity = await tableOperation.InsertAsync(dbcustomerActivity);
            dbcustomerActivity.Id = dbcustomerActivity.RowKey;
        }
        else
        {
            dbcustomerActivity = await tableOperation.AddOrUpdateAsync(dbcustomerActivity);
        }

        return Helpers.Clone<CustomerActivity, TEntity>(dbcustomerActivity);
    }

    public async Task DeleteAsync(string id)
    {
        await tableOperation.DeleteAsync(id);
    }
}
