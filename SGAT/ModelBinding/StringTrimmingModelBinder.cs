using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace SGAT.Web.ModelBinding
{
    public sealed class StringTrimmingModelBinder : IModelBinder
    {
        public Task BindModelAsync(
            ModelBindingContext bindingContext)
        {
            ArgumentNullException.ThrowIfNull(bindingContext);

            var valueResult = bindingContext.ValueProvider
                .GetValue(bindingContext.ModelName);

            if (valueResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(
                bindingContext.ModelName,
                valueResult);

            var value = valueResult.FirstValue;

            if (DebePreservarValor(bindingContext))
            {
                bindingContext.Result =
                    ModelBindingResult.Success(value);

                return Task.CompletedTask;
            }

            if (string.IsNullOrEmpty(value))
            {
                bindingContext.Result =
                    ModelBindingResult.Success(value);

                return Task.CompletedTask;
            }

            var valorRecortado = value.Trim();

            bindingContext.Result =
                ModelBindingResult.Success(
                    string.IsNullOrEmpty(valorRecortado)
                        ? null
                        : valorRecortado);

            return Task.CompletedTask;
        }

        private static bool DebePreservarValor(
            ModelBindingContext bindingContext)
        {
            var metadata = bindingContext.ModelMetadata;

            if (string.Equals(
                metadata.DataTypeName,
                "Password",
                StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (metadata is DefaultModelMetadata defaultMetadata &&
                defaultMetadata.Attributes.Attributes
                    .OfType<PreserveStringAttribute>()
                    .Any())
            {
                return true;
            }

            return bindingContext.ModelName.Contains(
                "Password",
                StringComparison.OrdinalIgnoreCase);
        }
    }

    public sealed class StringTrimmingModelBinderProvider
        : IModelBinderProvider
    {
        private readonly StringTrimmingModelBinder _binder = new();

        public IModelBinder? GetBinder(
            ModelBinderProviderContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (context.Metadata.ModelType == typeof(string))
            {
                return _binder;
            }

            return null;
        }
    }
}