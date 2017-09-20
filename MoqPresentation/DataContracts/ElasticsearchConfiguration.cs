using System;
namespace MoqPresentation.DataContracts
{
    /// <summary>
    /// Elasticsearch configuration.
    /// </summary>
    public class ElasticsearchConfiguration
    {
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>The address.</value>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the elasticsearch time out in seconds.
        /// </summary>
        /// <value>The elasticsearch time out in seconds.</value>
        public int TimeOutInSeconds { get; set; }

        /// <summary>
        /// Gets or sets the name of the index.
        /// </summary>
        /// <value>The name of the index.</value>
        public string IndexName { get; set; }
    }
}
