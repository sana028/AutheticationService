using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NetCoreIntermediate.DbContextService;
using NetCoreIntermediate.Interfaces;

namespace NetCoreIntermediate.Repositories
{
    public class BaseCrudServiceRepository<T,Type> : IBaseCrudService<T, Type> where T : class
    {
        private readonly AuthenticationDbContext AuthenticationDb;
        private DbSet<T> Set { get; set; }
        private readonly IMapper Mapper; 
        public BaseCrudServiceRepository(AuthenticationDbContext trackerDbContext, IMapper mapper)
        {
            AuthenticationDb = trackerDbContext;
            Set = AuthenticationDb.Set<T>();
            Mapper = mapper;
        }
        public async Task<(bool isSuccess,string message)> AddNewData(T data)
        {
            try
            {
               if(data == null)
                {
                    return (false, "Data can not be null");
                }
                await Set.AddAsync(data);
                await AuthenticationDb.SaveChangesAsync();
                return (true, "Data Added successfully");
            }
            catch(Exception ex) 
            {
                return (false, ex.Message);
            }
        }

        public async Task<string> DeleteTheData(Type id)
        {
            if (id == null)
            {
                return "Id is null";
            }

            try
            {
                var deleteItem = await Set.FindAsync(id);
                if (deleteItem == null)
                {
                    return "Item not found";
                }

                Set.Remove(deleteItem);
                await AuthenticationDb.SaveChangesAsync();
                return "Data deleted successfully";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<T> GetTheData(Type id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id), "Id cannot be null");
            }

            try
            {
                var data = await Set.FindAsync(id); // FindAsync returns null if the item is not found
                return data; // If not found, null is returned, which is acceptable here
            }
            catch (TaskCanceledException ex)
            {
                throw new Exception($"An error occurred while fetching data: {ex.Message}", ex);
            }
        }


        public async Task UpdateTheData(Type id,T data)
        {
           
            try
            {
                var existingData = await Set.FindAsync(id);
                if(existingData == null)
                {
                    throw new Exception($"No data present with this id");
                }
                Mapper.Map(existingData, data);
                await AuthenticationDb.SaveChangesAsync();
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
