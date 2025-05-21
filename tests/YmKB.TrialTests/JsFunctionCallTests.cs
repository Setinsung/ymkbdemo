using System.Text.Json;
using Microsoft.SemanticKernel;
using YmKB.JSFunctionCall;

namespace YmKB.TrialTests;

public class JsFunctionCallAsyncTest : IAsyncLifetime
{
    private JsFunctionCallContext _context;

    public async Task InitializeAsync()
    {
        _context = new JsFunctionCallContext();
    }

    public async Task DisposeAsync()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task CommonTest()
    {
        List<int> test =  [ 1, 2 ];
        object[] objtest = test.ToArray().Cast<object>().ToArray();

        RunFunc(test);
        RunFunc(objtest);

        RunFunc2(new dynamic[] { 1, 2, "2" });

        void RunFunc(params object[] args)
        {
            Console.WriteLine(args);
            Console.WriteLine(args[0]);
            Console.WriteLine(JsonSerializer.Serialize(args));
        }

        void RunFunc2(params dynamic[] args)
        {
            Console.WriteLine(args);
            Console.WriteLine(args[0]);
            Console.WriteLine(JsonSerializer.Serialize(args));
        }
    }

    [Fact]
    public async Task SKFunctionCallAsync()
    {
        var kernel = Kernel
            .CreateBuilder()
            .AddOpenAIChatCompletion(
                "",
                endpoint: new(""),
                ""
            )
            .Build();
        // var f = async (params dynamic[] args) =>
        // {
        //     string script = @"
        //             function add(x,y)
        //             {
        //              return x+y+1000000
        //             }
        //         ";
        //     var result = await _context.FunctionCallAsync(script,
        //         "add",
        //         args);
        //     return result;
        // };
        var function = kernel.CreateFunctionFromMethod(
            async (params dynamic[] args) =>
            {
                string script =
                    @"
                    function add(x,y)
                    {
	                    return x+y+1000000
                    }
                ";
                var result = await _context.FunctionCallAsync(script, "add", args);
                return result;
            },
            "add",
            "魔法加法",
            [
                new KernelParameterMetadata("x"){Name = "x",Description = "第一个数字"},
                new KernelParameterMetadata("y"){Name = "y",Description = "第二个数字"}
            ]
        );
        var res = await function.InvokeAsync(kernel, new() { ["args"] = new object[] { 1, 2 } });
        var intres = int.Parse(res.ToString());
        Assert.Equal(1000003, intres);
    }

    [Fact]
    public async Task FunctionCallAsync_ShouldExecuteScriptAndInvokeFunction()
    {
        string script =
            @"
                function add(a, b) {
                    return a + b + 10000;
                }
            ";
        string functionName = "add";
        object[] args = { (object)1, (object)2 };

        var result = await _context.FunctionCallAsync(script, functionName, args);

        Assert.Equal(3, result);
    }

    [Fact]
    public async Task FunctionCallAsync_WithPromise_ShouldAwaitAndReturnResult()
    {
        string script =
            @"
                function asyncFunction() {
                    return new Promise((resolve) => {
                        setTimeout(() => {
                            resolve(42);
                        }, 100);
                    });
                }
            ";
        string functionName = "asyncFunction";
        object[] args = { };

        var result = await _context.FunctionCallAsync(script, functionName, args);

        Assert.Equal(42, result);
    }

    [Fact]
    public async Task FunctionCallAsync_WithHttpClientHelper_GetAsync()
    {
        string script =
            @"
                async function callGet() {
                    return await HttpClientHelper.GetAsync('https://jsonplaceholder.typicode.com/todos/1');
                }
            ";
        string functionName = "callGet";
        object[] args = { };

        var result = await _context.FunctionCallAsync(script, functionName, args);

        Assert.NotNull(result);
        Assert.IsType<string>(result);
    }

    [Fact]
    public async Task FunctionCallAsync_WithHttpClientHelper_PostAsync()
    {
        string script =
            @"
                async function callPost() {
                    const data = { title: 'foo', body: 'bar', userId: 1 };
                    return await HttpClientHelper.PostAsync('https://jsonplaceholder.typicode.com/posts', data);
                }
            ";
        string functionName = "callPost";
        object[] args = { };

        var result = await _context.FunctionCallAsync(script, functionName, args);

        Assert.NotNull(result);
        Assert.IsType<string>(result);
    }

    [Fact]
    public async Task FunctionCallAsync_WithHttpClientHelper_PutAsync()
    {
        string script =
            @"
                async function callPut() {
                    const data = { id: 1, title: 'foo', body: 'bar', userId: 1 };
                    return await HttpClientHelper.PutAsync('https://jsonplaceholder.typicode.com/posts/1', data);
                }
            ";
        string functionName = "callPut";
        object[] args = { };

        var result = await _context.FunctionCallAsync(script, functionName, args);

        Assert.NotNull(result);
        Assert.IsType<string>(result);
    }

    [Fact]
    public async Task FunctionCallAsync_WithHttpClientHelper_DeleteAsync()
    {
        string script =
            @"
                async function callDelete() {
                    return await HttpClientHelper.DeleteAsync('https://jsonplaceholder.typicode.com/posts/1');
                }
            ";
        string functionName = "callDelete";
        object[] args = { };

        var result = await _context.FunctionCallAsync(script, functionName, args);

        Assert.NotNull(result);
        Assert.IsType<string>(result);
    }
}
