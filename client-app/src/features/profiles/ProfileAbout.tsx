import { observer } from "mobx-react-lite";
import React from "react";
import { Profile } from "../../app/models/profile";

interface Props {
    profile: Profile;
}

export default observer(function ProfileAbout({ profile }: Props) {
    return (
        <></>
    )
})