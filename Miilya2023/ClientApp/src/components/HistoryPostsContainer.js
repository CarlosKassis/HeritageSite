import React, { useEffect, useState, useRef } from 'react';
import MyAPI from '../MyAPI';
import { CreateHistoryPost } from './CreateHistoryPost';
import { HistoryPost } from './HistoryPost';

export function HistoryPostsContainer({ loginInfo, loadMoreFlag, onLoadingStop }) {

    const historyPostsRef = useRef(null);
    const [historyPosts, setHistoryPosts] = useState([]);

    // Initial history posts load
    useEffect(() => {
        tryLoadMorePosts();
    }, [loginInfo, loadMoreFlag]);

    // To keep everything in sync
    function setHistoryPostsVariables(newHistoryPosts) {
        setHistoryPosts(newHistoryPosts);
        historyPostsRef.current = newHistoryPosts;
    }

    function tryLoadMorePosts() {
        if (!loginInfo) {
            return;
        }

        if (!loginInfo.loggedIn) {
            return;
        }

        var startingFromIndex = null;
        if (historyPostsRef.current != null && historyPostsRef.current.length > 0) {
            startingFromIndex = Math.min(...historyPostsRef.current.map(historyPost => historyPost.Index)) - 1;
        }

        if (loginInfo.loggedIn) {
            MyAPI.getHistoryPosts(loginInfo.jwt, startingFromIndex).then(historyPostsResponse => {
                onLoadingStop();
                if (historyPostsResponse) {
                    if (historyPostsResponse.length == 0) {
                        console.log('No more posts in site')
                        return;
                    }

                    console.log(historyPostsResponse);

                    // Use Ref instead of State History Posts because this function could be called from callback
                    // And there's no access to State variables from callback since the callback captures only
                    // The initial value of the State variable
                    if (historyPostsRef.current) {
                        // Keep track of which posts were already fetched
                        const setOfHistoryPostsIndexes = new Set(historyPostsRef.current.map(historyPost => historyPost.Index));

                        // Create new array with new reference
                        var newCombinedHistoryPosts = [].concat(historyPostsRef.current);

                        console.log(newCombinedHistoryPosts);
                        for (const historyPost of historyPostsResponse) {

                            if (!(historyPost.Index in setOfHistoryPostsIndexes)) {
                                newCombinedHistoryPosts.push(historyPost);
                            }
                        }

                        console.log('Consecutive load!')
                        setHistoryPostsVariables(newCombinedHistoryPosts);
                    } else {
                        console.log('First load!')
                        setHistoryPostsVariables(historyPostsResponse);
                    }
                }
            }).catch(ex => {
                onLoadingStop();
            });
        }
    }

    return (
        <div className={"history-posts-container"}>
            <CreateHistoryPost loginInfo={loginInfo} />
            {
                historyPosts.map((historyPost) => (
                    <HistoryPost
                        key={historyPost.Index}
                        index={historyPost.Index}
                        imageName={historyPost.ImageName}
                        title={historyPost.Title}
                        description={historyPost.Description}
                        myPost={historyPost.MyPost}
                        bookmarked={historyPost.Bookmarked}
                        loginInfo={loginInfo}
                    />
                ))
            }
        </div>
    );
}