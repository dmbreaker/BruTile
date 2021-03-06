﻿// Copyright (c) BruTile developers team. All rights reserved. See License.txt in the project root for license information.

using System;
using BruTile.Cache;

namespace BruTile.Web
{
    public class WebTileProvider : ITileProvider, IRequest
    {
        private readonly Func<Uri, byte[]> _fetchTile;
        private readonly IPersistentCache<byte[]> _persistentCache;
        private readonly IRequest _request;

        public WebTileProvider(IRequest request = null, IPersistentCache<byte[]> persistentCache = null,
            Func<Uri, byte[]> fetchTile = null)
        {
            _request = request ?? new NullRequest();
            _persistentCache = persistentCache ?? new NullCache();
            _fetchTile = fetchTile ?? (RequestHelper.FetchImage);
        }

        public IPersistentCache<byte[]> PersistentCache
        {
            get { return _persistentCache; }
        }

        public Uri GetUri(TileInfo tileInfo)
        {
            return _request.GetUri(tileInfo);
        }

        public byte[] GetTile(TileInfo tileInfo)
        {
            var bytes = PersistentCache.Find(tileInfo.Index);
            if (bytes == null)
            {
                bytes = _fetchTile(_request.GetUri(tileInfo));
                if (bytes != null) PersistentCache.Add(tileInfo.Index, bytes);
            }
            return bytes;
        }
    }
}
