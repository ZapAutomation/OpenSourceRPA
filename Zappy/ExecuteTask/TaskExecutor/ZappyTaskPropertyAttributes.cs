namespace Zappy.ExecuteTask.TaskExecutor
{
    public enum ZappyTaskPropertyAttributes
    {
        CommonToTechnology = 0x20,
        DoNotGenerateProperties = 0x10,

        NonAssertable = 8,
        None = 0,
        Readable = 1,
        Searchable = 4,
        Writable = 2
    }
}