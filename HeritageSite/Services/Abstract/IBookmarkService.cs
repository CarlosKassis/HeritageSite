﻿

namespace HeritageSite.Services.Abstract
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using static HeritageSite.Services.Utils.Documents;

    public interface IBookmarkService
    {
        public Task<IEnumerable<BookmarkDocument>> GetUserBookmarks(UserDocument user);

        public Task AddBookmark(UserDocument user, int historyPostIndex);

        public Task RemoveBookmark(UserDocument user, int historyPostIndex);

        public Task DeleteBookmarksForHistoryPost(int historyPostIndex);
    }
}
