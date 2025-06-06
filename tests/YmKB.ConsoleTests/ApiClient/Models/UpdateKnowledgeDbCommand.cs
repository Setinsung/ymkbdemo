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
    public partial class UpdateKnowledgeDbCommand : IAdditionalDataHolder, IParsable
    #pragma warning restore CS1591
    {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The chatModelID property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? ChatModelID { get; set; }
#nullable restore
#else
        public string ChatModelID { get; set; }
#endif
        /// <summary>The description property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Description { get; set; }
#nullable restore
#else
        public string Description { get; set; }
#endif
        /// <summary>The embeddingModelID property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? EmbeddingModelID { get; set; }
#nullable restore
#else
        public string EmbeddingModelID { get; set; }
#endif
        /// <summary>The icon property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Icon { get; set; }
#nullable restore
#else
        public string Icon { get; set; }
#endif
        /// <summary>The id property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Id { get; set; }
#nullable restore
#else
        public string Id { get; set; }
#endif
        /// <summary>The maxTokensPerLine property</summary>
        public int? MaxTokensPerLine { get; set; }
        /// <summary>The maxTokensPerParagraph property</summary>
        public int? MaxTokensPerParagraph { get; set; }
        /// <summary>The name property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Name { get; set; }
#nullable restore
#else
        public string Name { get; set; }
#endif
        /// <summary>The overlappingTokens property</summary>
        public int? OverlappingTokens { get; set; }
        /// <summary>The tags property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public List<string>? Tags { get; set; }
#nullable restore
#else
        public List<string> Tags { get; set; }
#endif
        /// <summary>
        /// Instantiates a new <see cref="global::YMKB.ConsoleTests.Client.Models.UpdateKnowledgeDbCommand"/> and sets the default values.
        /// </summary>
        public UpdateKnowledgeDbCommand()
        {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <returns>A <see cref="global::YMKB.ConsoleTests.Client.Models.UpdateKnowledgeDbCommand"/></returns>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static global::YMKB.ConsoleTests.Client.Models.UpdateKnowledgeDbCommand CreateFromDiscriminatorValue(IParseNode parseNode)
        {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new global::YMKB.ConsoleTests.Client.Models.UpdateKnowledgeDbCommand();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        /// <returns>A IDictionary&lt;string, Action&lt;IParseNode&gt;&gt;</returns>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
        {
            return new Dictionary<string, Action<IParseNode>>
            {
                { "chatModelID", n => { ChatModelID = n.GetStringValue(); } },
                { "description", n => { Description = n.GetStringValue(); } },
                { "embeddingModelID", n => { EmbeddingModelID = n.GetStringValue(); } },
                { "icon", n => { Icon = n.GetStringValue(); } },
                { "id", n => { Id = n.GetStringValue(); } },
                { "maxTokensPerLine", n => { MaxTokensPerLine = n.GetIntValue(); } },
                { "maxTokensPerParagraph", n => { MaxTokensPerParagraph = n.GetIntValue(); } },
                { "name", n => { Name = n.GetStringValue(); } },
                { "overlappingTokens", n => { OverlappingTokens = n.GetIntValue(); } },
                { "tags", n => { Tags = n.GetCollectionOfPrimitiveValues<string>()?.AsList(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer)
        {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteStringValue("chatModelID", ChatModelID);
            writer.WriteStringValue("description", Description);
            writer.WriteStringValue("embeddingModelID", EmbeddingModelID);
            writer.WriteStringValue("icon", Icon);
            writer.WriteStringValue("id", Id);
            writer.WriteIntValue("maxTokensPerLine", MaxTokensPerLine);
            writer.WriteIntValue("maxTokensPerParagraph", MaxTokensPerParagraph);
            writer.WriteStringValue("name", Name);
            writer.WriteIntValue("overlappingTokens", OverlappingTokens);
            writer.WriteCollectionOfPrimitiveValues<string>("tags", Tags);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}
#pragma warning restore CS0618
