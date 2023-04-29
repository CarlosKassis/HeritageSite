import React, { useEffect, useRef } from 'react';
import MyAPI from '../MyAPI';
import FamilyTree from "@balkangraph/familytree.js";

export function BalkanFamilyTreeWrapper({ loginInfo, familyId }) {

    const treeHolderRef = useRef(null);

    useEffect(() => {
        if (!loginInfo.loggedIn) {
            return;
        }

        MyAPI.getFamilyTree(loginInfo.jwt, familyId).then(familyTree => {
            new FamilyTree(treeHolderRef.current, {
                nodes: familyTree,
                scaleInitial: 1,
                levelSeparation: 30,
                //template: "hugo",
                nodeBinding: {
                    field_0: 'name',
                    img_0: 'img'
                }
            });
        });
    }, [loginInfo]);

    return (
        <div id={"tree"} className={"arabic"} style={{ direction: 'ltr' }} ref={treeHolderRef} ></div>
    );
}