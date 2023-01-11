using DataServices.Models;

namespace DataServices.Services;

public class CustomerService
{
    public readonly string customerTableName = "Customer";
    private readonly DataTableBaseOperation<Customer> customerTableOperation;

    public CustomerService(string connectionString)
    {
        customerTableOperation = new(connectionString, customerTableName);
    }

    public async Task<List<TEntity>> GetCustomers<TEntity>(string id) where TEntity : class
    {

        List<TEntity> customers = new();
        if (string.IsNullOrEmpty(id))
        {
            var data = await customerTableOperation.QueryAllAsync();
            await foreach (var item in data)
            {
                item.Id = item.RowKey;
                customers.Add(Helpers.Clone<Customer, TEntity>(item));
            }
        } else
        {
            var item = await customerTableOperation.GetAsync(id);
            item.Id = item.RowKey;
            customers.Add(Helpers.Clone<Customer, TEntity>(item));
        }

        return customers;
    }

    public async Task<TEntity> AddOrUpdateCustomerAsync<TEntity>(TEntity customer) where TEntity : class
    {
        var dbcustomer = Helpers.Clone<TEntity, Customer>(customer);
        dbcustomer.RowKey = dbcustomer.Id;

        if (string.IsNullOrEmpty(dbcustomer.RowKey))
        {
            dbcustomer = await customerTableOperation.InsertAsync(dbcustomer);
            dbcustomer.Id = dbcustomer.RowKey;
        } else
        {
            dbcustomer = await customerTableOperation.AddOrUpdateAsync(dbcustomer);
        }

        return Helpers.Clone<Customer, TEntity>(dbcustomer);
    }

    public async Task DeleteAsync(string id)
    {
        await customerTableOperation.DeleteAsync(id);
    }

}