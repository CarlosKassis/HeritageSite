﻿

namespace Miilya2023.Services.Abstract
{
    using System.Threading.Tasks;
    using static Miilya2023.Services.Utils.Documents;

    public interface IBookmarkService
    {
        public Task<BookmarkDocument> GetUserBookmarks(UserDocument user);

        public Task AddBookmark(UserDocument user, int historyPostIndex);

        public Task RemoveBookmark(UserDocument user, int historyPostIndex);
    }
}
