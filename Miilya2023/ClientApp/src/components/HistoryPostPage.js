import React, { useEffect, useState, useRef } from 'react';
import LocalizedStrings from 'localized-strings';
import MyAPI from '../MyAPI';
import { HistoryPost } from './HistoryPost';

export function HistoryPostPage(props) {

    const historyPostsRef = useRef(null);
    const [historyPosts, setHistoryPosts] = useState([]);

    const strings = new LocalizedStrings({
        ar: {
            families: 'عائلات'
        },
        he: {
            families: 'משפחות'
        },
    });

    function getString(language, str) {
        strings.setLanguage(language);
        return strings[str];
    }

    // Initial history posts load
    useEffect(() => {
        tryLoadMorePosts();
    }, [props.loginInfo]);

    // To keep everything in sync
    function setHistoryPostsVariables(newHistoryPosts) {
        setHistoryPosts(newHistoryPosts);
        historyPostsRef.current = newHistoryPosts;
    }

    // Reset if reched page bottom
    const loadPostsCoolingDownFlag = useRef(false);
    const scrollBottomMargin = 5;

    function checkIfPageBottomAndLoadMorePosts() {
        const scrollTop = window.pageYOffset;
        const maxScroll = document.documentElement.scrollHeight - window.innerHeight;

        if (scrollTop >= maxScroll - scrollBottomMargin) {
            tryLoadMorePosts();
        }
    }

    function tryLoadMorePosts() {
        if (loadPostsCoolingDownFlag.current) {
            return;
        }

        if (!props.loginInfo.loggedIn) {
            return;
        }

        var startingFromIndex = null;
        if (historyPostsRef.current != null && historyPostsRef.current.length > 0) {
            startingFromIndex = Math.max(...historyPostsRef.current.map(historyPost => historyPost.Index)) + 1;
        }

        if (props.loginInfo.loggedIn) {
            MyAPI.getHistoryPosts(props.loginInfo.jwt, startingFromIndex).then(historyPostsResponse => {
                if (historyPostsResponse) {
                    if (historyPostsResponse.length == 0) {
                        console.log('No more posts in site')
                        return;
                    }

                    // Use Ref instead of State History Posts because this function could be called from callback
                    // And there's no access to State variables from callback since the callback captures only
                    // The initial value of the State variable
                    if (historyPostsRef.current) {
                        // Keep track of which posts were already fetched
                        const setOfHistoryPostsIndexes = new Set(historyPostsRef.current.map(historyPost => historyPost.Index));
                        console.log(setOfHistoryPostsIndexes);

                        // Create new array with new reference
                        var newCombinedHistoryPosts = [].concat(historyPostsRef.current);

                        console.log(newCombinedHistoryPosts);
                        for (const historyPost of historyPostsResponse) {

                            if (!(historyPost.Index in setOfHistoryPostsIndexes)) {
                                console.log('added ' + historyPost.Index)
                                newCombinedHistoryPosts.push(historyPost);
                            } else {
                                console.log('Not added ' + historyPost.Index)
                            }
                        }

                        console.log('consecutive load!!')
                        setHistoryPostsVariables(newCombinedHistoryPosts);
                    } else {
                        console.log('first load!!')
                        setHistoryPostsVariables(historyPostsResponse);
                    }

                    // Cooldown loading posts
                    if (!loadPostsCoolingDownFlag.current) {
                        console.log('cooldown load')
                        loadPostsCoolingDownFlag.current = true;

                        setTimeout(() => {
                            console.log('reset cooldown')
                            checkIfPageBottomAndLoadMorePosts();
                            loadPostsCoolingDownFlag.current = false;
                        }, 1000)
                    }
                }
            });
        }
    }

    return (
        <div onTouchMove={(e) => checkIfPageBottomAndLoadMorePosts()} onWheel={(e) => checkIfPageBottomAndLoadMorePosts()} style={{
            alignContent: 'center',
            display: 'block',
            overflow: 'hidden'
        }}>
            {
                historyPosts.map((historyPost) => (
                    <HistoryPost
                        key={historyPost.Index}
                        index={historyPost.Index}
                        imageName={historyPost.ImageName}
                        title={historyPost.Title}
                        description={historyPost.Description}
                        loginInfo={props.loginInfo}
                    />
                ))
            }
            <div style={{ height: '20vh' }} />
        </div>
    );
}