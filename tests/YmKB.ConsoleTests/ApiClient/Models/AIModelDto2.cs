// <auto-generated/>
#pragma warning disable CS0618
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System;
namespace YMKB.ConsoleTests.Client.Models
{
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    #pragma warning disable CS1591
    public partial class AIModelDto2 : IAdditionalDataHolder, IParsable
    #pragma warning restore CS1591
    {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The aiModelType property</summary>
        public global::YMKB.ConsoleTests.Client.Models.AIModelType? AiModelType { get; set; }
        /// <summary>The endpoint property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Endpoint { get; set; }
#nullable restore
#else
        public string Endpoint { get; set; }
#endif
        /// <summary>The id property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Id { get; set; }
#nullable restore
#else
        public string Id { get; set; }
#endif
        /// <summary>The modelDescription property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? ModelDescription { get; set; }
#nullable restore
#else
        public string ModelDescription { get; set; }
#endif
        /// <summary>The modelKey property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? ModelKey { get; set; }
#nullable restore
#else
        public string ModelKey { get; set; }
#endif
        /// <summary>The modelName property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? ModelName { get; set; }
#nullable restore
#else
        public string ModelName { get; set; }
#endif
        /// <summary>
        /// Instantiates a new <see cref="global::YMKB.ConsoleTests.Client.Models.AIModelDto2"/> and sets the default values.
        /// </summary>
        public AIModelDto2()
        {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::YMKB.ConsoleTests.Client.Models.AIModelDto2"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::YMKB.ConsoleTests.Client.Models.AIModelDto2 CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::YMKB.ConsoleTests.Client.Models.AIModelDto2();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "aiModelType", n => { AiModelType = n.GetEnumValue<global::YMKB.ConsoleTests.Client.Models.AIModelType>(); } },
                { "endpoint", n => { Endpoint = n.GetStringValue(); } },
                { "id", n => { Id = n.GetStringValue(); } },
                { "modelDescription", n => { ModelDescription = n.GetStringValue(); } },
                { "modelKey", n => { ModelKey = n.GetStringValue(); } },
                { "modelName", n => { ModelName = n.GetStringValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteEnumValue<global::YMKB.ConsoleTests.Client.Models.AIModelType>("aiModelType", AiModelType);
            writer.WriteStringValue("endpoint", Endpoint);
            writer.WriteStringValue("id", Id);
            writer.WriteStringValue("modelDescription", ModelDescription);
            writer.WriteStringValue("modelKey", ModelKey);
            writer.WriteStringValue("modelName", ModelName);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}
#pragma warning restore CS0618
