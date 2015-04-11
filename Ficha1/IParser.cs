
namespace Ficha1
{
    interface IParser<T>
    {
        T Parse(string[] args);
        //bool HasMandatoryArgs(T parseResult, string[] keys);
    }
}

