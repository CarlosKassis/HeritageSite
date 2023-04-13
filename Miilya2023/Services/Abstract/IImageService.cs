﻿
namespace Miilya2023.Services.Abstract
{
    using Miilya2023.Services.Concrete;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IImageService
    {
        public Task<List<ImageDocument>> GetFirstBatchGreaterThanIndex(int index, int batchSize = 20);
    }
}
