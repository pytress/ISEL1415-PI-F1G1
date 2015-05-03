namespace Ficha1
{
    interface IArgumentVerifier<T>
    {
        bool Verify(T[] keys);
    }
}
