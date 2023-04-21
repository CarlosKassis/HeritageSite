import React, { useState, useRef } from 'react';

export function InfinitePage(props) {

    const [loading, setLoading] = useState(false);
    const [loadMoreFlag, setLoadMoreFlag] = useState(0);

    const loadPostsCoolingDownFlag = useRef(false);
    const scrollBottomMargin = 300;

    function checkIfPageBottomAndLoadMorePosts() {
        // Check if hit page bottom
        const scrollTop = window.pageYOffset;
        const maxScroll = document.documentElement.scrollHeight - window.innerHeight;

        //console.log(scrollTop + ' ' + maxScroll);

        // Ignore if on load cooldown
        if (loadPostsCoolingDownFlag.current) {
            return;
        }

        if (scrollTop >= maxScroll - scrollBottomMargin) {
            setLoading(true);
            cooldownLoadingElements();
            setLoadMoreFlag(loadMoreFlag + 1);
        }
    }

    function cooldownLoadingElements() {
        // Cooldown loading posts
        console.log('cooldown load')
        loadPostsCoolingDownFlag.current = true;

        setTimeout(() => {
            console.log('reset cooldown')
            loadPostsCoolingDownFlag.current = false;
        }, 1000)
    }

    function onLoadingStop() {
        setLoading(false);
    }

    const previousPointerY = useRef(null);

    return (
        <div
            onTouchMove={(e) => {
                if (!previousPointerY.current) {
                    previousPointerY.current = e.touches[0].screenY;
                    return;
                }

                if (previousPointerY.current - e.touches[0].screenY < 0) {
                    checkIfPageBottomAndLoadMorePosts()
                }
            }}
            onWheel={(e) => {
                if (e.deltaY > 0) {
                    checkIfPageBottomAndLoadMorePosts()
                }
            }}
            style={{
                width: '100%',
                height:'100%'
                } }>
            
            {
                React.cloneElement(props.children, { loginInfo: props.loginInfo, loadMoreFlag: loadMoreFlag, onLoadingStop: onLoadingStop })
            }
            {
                // Show loading animation or leave space
                (loading &&
                    <img
                        src={"./loading.gif"} alt={"Loading more!!!"}
                        style={{
                            display: 'block',
                            marginLeft: 'auto',
                            marginRight: 'auto',
                            width: '12vh',
                            paddingBottom: '4vh'
                        }} />) ||
                (!loading && <div style={{ height:'10vh' }} />)
            }
        </div>
    );
}