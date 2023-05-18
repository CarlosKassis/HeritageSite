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
            FamilyTree.templates.myTemplate = Object.assign({}, FamilyTree.templates.tommy);
            FamilyTree.templates.myTemplate_male = Object.assign({}, FamilyTree.templates.myTemplate);
            FamilyTree.templates.myTemplate_male.node = '<circle cx="100" cy="100" r="100" fill="#039be5" stroke-width="1" stroke="#aeaeae"></circle>';
            FamilyTree.templates.myTemplate_female = Object.assign({}, FamilyTree.templates.myTemplate);
            FamilyTree.templates.myTemplate_female.node = '<circle cx="100" cy="100" r="100" fill="#FF46A3" stroke-width="1" stroke="#aeaeae"></circle>';


            const family = new FamilyTree(treeHolderRef.current, {
                nodes: familyTree,
                scaleInitial: 1,
                levelSeparation: 150,
                template: "myTemplate",
                //siblingSeparation: 5,
                //subtreeSeparation: 40,
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