using System;
using MoqPresentation.DataContracts;
using MoqPresentation.Model;
using Nest;

namespace MoqPresentation.Repositories
{
    public class UserElasticsearchRepository
    {

        #region Constructors

        /// <summary>
        /// Base constructor.
        /// </summary>
        public UserElasticsearchRepository()
        {
        }

        /// <summary>
        /// Constructor with configuration.
        /// </summary>
        /// <param name="config">Config.</param>
        public UserElasticsearchRepository(ElasticsearchConfiguration config)
        {
            Configuration = config;
            Uri uri = new Uri(config.Address);
            ConnectionSettings settings = new ConnectionSettings(uri: uri);
            settings.RequestTimeout(TimeSpan.FromSeconds(config.TimeOutInSeconds));
            settings.DefaultIndex(config.IndexName);
            settings.ThrowExceptions();
            Client = new ElasticClient(settings);

        }

        /// <summary>
        /// Constructor with client.
        /// </summary>
        /// <param name="client">Client.</param>
        public UserElasticsearchRepository(IElasticClient client)
        {
            this.Client = client;
        }

        #endregion

        #region Protected properties

        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        /// <value>The client.</value>
        protected IElasticClient Client { get; set; }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public ElasticsearchConfiguration Configuration { get; set; }

        #endregion

        #region Public methods

        /// <summary>
        /// Add the specified data.
        /// </summary>
        /// <returns>The add.</returns>
        /// <param name="data">Data.</param>
        public void Add(User data)
        {
            ICreateResponse response = Client.Create(data);
        }

        /// <summary>
        /// Get the specified id.
        /// </summary>
        /// <returns>The get.</returns>
        /// <param name="id">Identifier.</param>
        public User Get(int id)
        {
            IGetResponse<User> response;               
            IGetRequest request = new GetRequest(Configuration.IndexName, TypeName.From<User>(), new Id(id));
            response = Client.Get<User>(request);

            if(response.Found){
                return response.Source;
            }else{
                return null;
            }
        }

        #endregion
    }
}