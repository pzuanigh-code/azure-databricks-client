﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Databricks.Client
{
    public class InstancePoolApiClient : ApiClient, IInstancePoolApi
    {
        /// <inheritdoc />
        public InstancePoolApiClient(HttpClient httpClient) : base(httpClient)
        {
        }

        /// <inheritdoc />
        public async Task<string> Create(InstancePoolAttributes poolAttributes, CancellationToken cancellationToken = default)
        {
            var poolIdentifier =
                await HttpPost<InstancePoolAttributes, dynamic>(
                    this.HttpClient,
                    "instance-pools/create",
                    poolAttributes, cancellationToken).ConfigureAwait(false);
            return poolIdentifier.instance_pool_id.ToObject<string>();
        }

        /// <inheritdoc />
        public async Task Edit(string poolId, InstancePoolAttributes poolAttributes, CancellationToken cancellationToken = default)
        {
            var pool = new InstancePoolInfo
            {
                PoolId = poolId,
                PoolName = poolAttributes.PoolName,
                NodeTypeId = poolAttributes.NodeTypeId,
                MinIdleInstances = poolAttributes.MinIdleInstances,
                MaxCapacity = poolAttributes.MaxCapacity,
                IdleInstanceAutoTerminationMinutes = poolAttributes.IdleInstanceAutoTerminationMinutes
            };

            await HttpPost(this.HttpClient, "instance-pools/edit", pool, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task Delete(string poolId, CancellationToken cancellationToken = default)
        {
            await HttpPost(this.HttpClient, "instance-pools/delete", new {instance_pool_id = poolId}, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<InstancePoolInfo> Get(string poolId, CancellationToken cancellationToken = default)
        {
            var requestUri = $"instance-pools/get?instance_pool_id={poolId}";
            return await HttpGet<InstancePoolInfo>(this.HttpClient, requestUri, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<InstancePoolInfo>> List(CancellationToken cancellationToken = default)
        {
            const string requestUri = "instance-pools/list";
            var poolList = await HttpGet<dynamic>(this.HttpClient, requestUri, cancellationToken).ConfigureAwait(false);
            return PropertyExists(poolList, "instance_pools")
                ? poolList.instance_pools.ToObject<IEnumerable<InstancePoolInfo>>()
                : Enumerable.Empty<InstancePoolInfo>();
        }
    }
}
