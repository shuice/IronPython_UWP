## Introduction:
This project to make IronPython2 running in UWP, the version of IronPython2 is 2.7.11 released on date 2020-11-17

## Problem
IronPython2 does not support UWP by default because of the following reasons
1. Local files have been accessed
2. Partial reflection code
3. The `.Net Native Tool Chain` function in the Release version makes the program abnormal, including:

    1). Cannot load the appropriate type, because the Type.GetType() method returns a temporary type, and Type.GetType().GetType() is required to return the correct type
    
    2). When the Python script contains expression`for x in range(1,2)`, a MissingRuntimeArtifactException exception will be fired, as the content in url describes https://github.com/dotnet/corert/issues/7207.

## Solution
1. Delete the local access section
2. Delete some reflection code
3. Modify the entire IronPython source code, change `KeyValuePair<,>` to `MyKeyValuePair<,>` which type is `class` rather than `struct`, at the same time, modify `Dictionary<,>` , `IDictionary<,>` to `MyDictionary<, >` and `IMyDictionary<,>`...
4. Modify all the namespace `Microsoft.Script.*` in the DLR library to `AnyPrefix.Microsoft.Script.*` (this step can be omitted)


## note:
1. The `Default.rd.xml` in the UWP program does not need to add any item, keep as default.
2. Don't forget to copy `CurrentVersion.props`, `Directory.Build.props` and `Build` in the root directory to the corresponding .sln directory when compiling your own IronPython.
3. There may be minor adjustments in the source code, which are difficult to find out one by one. If you want to modify it yourself from the beginning, you can consider running it through Debug first, and then modify it according to step 3 above.
4. If you know the root cause of KeyValuePair cannot be used, or have a more convenient solution, welcome to submit issure for communication.

## Test part:
Here are some code I have tried:

For the newly created UWP program, write the following code in `MainPage.xaml.cs`, and test whether `Button_Call_Site_KeyValuePair` will crash
```
private void Button_Call_Site_KeyValuePair(object sender, RoutedEventArgs e)
{
    CallSite<Func<CallSite, Object, KeyValuePair<IEnumerator, IDisposable>>> cs
                = CallSite<Func<CallSite, Object, KeyValuePair<IEnumerator, IDisposable>>>.Create(new KeyValuePairCallSiteBinder());
    var r = cs.Target(cs, new object());
}

public class KeyValuePairCallSiteBinder: DynamicMetaObjectBinder
{
    public override DynamicMetaObject Bind(DynamicMetaObject target, DynamicMetaObject[] args)
    {
        throw new NotImplementedException();
    }

    public override T BindDelegate<T>(CallSite<T> site, object[] args)
    {
        T t = (T)(object)new Func<CallSite, object, KeyValuePair<IEnumerator, IDisposable>>(GetListEnumerator);
        return t;
    }

    private KeyValuePair<IEnumerator, IDisposable> GetListEnumerator(CallSite site, Object value)
    {
        return new KeyValuePair<IEnumerator, IDisposable>(new List<Object> {value }.GetEnumerator(), null);
    }
}
```
√ Without add any project/library reference

√ Only add a reference to `System.Dynamic.Runtime` in Nuget

× Only add a reference to prject `Microsoft.Dynamic` that used by IronPython, crash fired.
