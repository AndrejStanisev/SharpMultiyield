# SharpMultiyield

Attempt at abusing the async await mechanism to replicate iterator method functionality, with the ability to yield an IEnumerable<T> as well as T.
  
So an enumerable defined like this 

``` 
static async IConcatEnumerable<string> Test()
{
    for (var i = 1; i < 5; i++)
    {
        await Yield.Single($"single{i}");
    }

    await Yield.Multiple(new[] { "multiple1", "multiple2", "multiple3" });
    return default;        
}
````

will contain the following elements:

```
"single1", "single2", "single3", "single4", "multiple1", "multiple2", "multiple3"

