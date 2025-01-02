namespace NetCoreIntermediate.Interfaces
{
    public interface IBaseCrudService<T, Type> where T : class
    {
        Task<(bool isSuccess, string message)> AddNewData(T data);
        Task<string> DeleteTheData(Type id);
        Task<T> GetTheData(Type id);
        Task UpdateTheData(Type id,T data);
           
    }
}
