using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace YmKB.API.Converters;

public class NullableEnumModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        var value = valueProviderResult.FirstValue;
        if (string.IsNullOrEmpty(value))
        {
            bindingContext.Result = ModelBindingResult.Success(null);
            return Task.CompletedTask;
        }

        if (Enum.TryParse(bindingContext.ModelType.GenericTypeArguments[0], value, out var result))
        {
            bindingContext.Result = ModelBindingResult.Success(result);
        }
        else
        {
            bindingContext.Result = ModelBindingResult.Failed();
        }

        return Task.CompletedTask;
    }
}

public class NullableEnumModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType.IsGenericType &&
            context.Metadata.ModelType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
            context.Metadata.ModelType.GenericTypeArguments[0].IsEnum)
        {
            return new NullableEnumModelBinder();
        }

        return null;
    }
}