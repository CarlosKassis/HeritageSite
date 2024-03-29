﻿import React, { useEffect, useState, useRef } from 'react';
import MyAPI from '../MyAPI';
import { CreateHistoryPost } from './CreateHistoryPost';
import { HistoryPost } from './HistoryPost';

export function HistoryPostsContainer({ loginInfo, loadMoreFlag, onLoadingStop, getImageUrl }) {

    const historyPostsRef = useRef(null);
    const [historyPosts, setHistoryPosts] = useState([]);
    const [onlyBookmarks, setOnlyBookmarks] = useState(false);
    const [searchText, setSearchText] = useState(null);
    const [viewMode, setViewMode] = useState("vertical");

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
            MyAPI.getHistoryPosts(loginInfo.jwt, startingFromIndex, searchText).then(historyPostsResponse => {
                onHistoryPostsResponse(historyPostsResponse);
            }).catch(ex => {
                onLoadingStop();
            });
        }
    }

    function onHistoryPostsResponse(historyPostsResponse) {
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
    }

    function onDeletePost(index) {
        const newHistoryPosts = [...historyPosts].filter(historyPost => historyPost.Index != index);
        setHistoryPostsVariables(newHistoryPosts);
    }

    const searchStamp = useRef(0);

    function onChangeSearch(e) {
        // Change stamp to make other timeout stamps irrelevant
        searchStamp.current++
        // Avoid overflow (somehow)
        searchStamp.current = searchStamp.current > 1000000 ? 0 : searchStamp.current; 

        const stamp = searchStamp.current;
        const text = e.target.value;
        setTimeout(() => { 
            onCooldownSearchType(stamp, text);
        }, 1000)
    }

    function onCooldownSearchType(stamp, text) {
        if (stamp != searchStamp.current) {
            return;
        }

        setSearchText(text);

        // Reset posts
        setHistoryPostsVariables([]);

        // Query posts with search text
        MyAPI.getHistoryPosts(loginInfo.jwt, null, text).then(historyPostsResponse => {
            onHistoryPostsResponse(historyPostsResponse);
        }).catch(ex => {
            onLoadingStop();
        });
    }

    function onClickBookmarkPost(index) {
        const historyPost = historyPostsRef.current.find(historyPost => historyPost.Index == index);
        if (!historyPost) {
            return;
        }

        if (!historyPost.Bookmarked) {
            MyAPI.bookmarkHistoryPost(loginInfo.jwt, index).then(() => {
                changeBookmarked(index, true);
            }).catch(ex => {
                console.log(ex);
            })
        }
        else {
            MyAPI.unbookmarkHistoryPost(loginInfo.jwt, index).then(() => {
                changeBookmarked(index, false);
            }).catch(ex => {
                console.log(ex);
            })
        }
    }

    function changeBookmarked(index, bookmarked) {
        const newHistoryPosts = [...historyPostsRef.current];
        const historyPost = newHistoryPosts.find(post => post.Index == index);
        historyPost.Bookmarked = bookmarked;
        setHistoryPostsVariables(newHistoryPosts);
    }

    return (
        <div className={"history-posts-container"}>
            <CreateHistoryPost loginInfo={loginInfo} />
            <div className={"card"}
                style={{
                    marginLeft: 'auto',
                    marginRight: 'auto',
                    marginTop: '1rem',
                    marginBottom: '1rem',
                    display: 'grid',
                    gridTemplateColumns: '0fr 0fr 0fr',
                    gridGap: '10px',
                    padding: '12px',
                    width: 'fit-content',
                    height: 'fit-content'
                }} >
                <input onChange={onChangeSearch} className={"floating"} style={{ maxWidth: '400px' }}></input>
                <img style={{ height: '32px' }} className={"history-button"} src={onlyBookmarks ? './bookmarked.png' : './bookmark.png'} onClick={() => setOnlyBookmarks(!onlyBookmarks)} />
                <img style={{ height: '32px' }} className={"history-button"} src={viewMode == "grid" ? './grid.png' : './vertical.png'} onClick={() => setViewMode(viewMode == "grid" ? "vertical" : "grid")} />
            </div>
                <div 
                    style={
                        viewMode == "grid" ?
                        {
                            display: 'grid',
                            gridTemplateColumns: 'repeat(auto-fit, 200px)',
                            justifyContent: 'center',
                            gridGap: '0.25rem'
                        } :
                        {}
                    }>
                {
                    historyPosts.filter((historyPost) => !onlyBookmarks || historyPost.Bookmarked).map((historyPost) => (
                        <HistoryPost
                            bookmarked={historyPost.Bookmarked}
                            onClickBookmarkPost={onClickBookmarkPost}
                            containerViewMode={viewMode}
                            getImageUrl={getImageUrl}
                            key={historyPost.Index}
                            index={historyPost.Index}
                            imageName={historyPost.ImageName}
                            imageDate={historyPost.ImageDate}
                            title={historyPost.Title}
                            description={historyPost.Description}
                            control={historyPost.Control}
                            initialBookmarkState={historyPost.Bookmarked}
                            onDeletePost={onDeletePost}
                            loginInfo={loginInfo}
                        />
                    ))
                }
            </div>
        </div>
    );
}