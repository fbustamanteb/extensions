﻿import * as React from 'react'
import * as QueryString from 'query-string'
import { RestLogEntity, RestApiKeyEntity } from './Signum.Entities.Rest'
import { EntitySettings, ViewPromise } from "../../../Framework/Signum.React/Scripts/Navigator";
import * as Navigator from "../../../Framework/Signum.React/Scripts/Navigator";
import { ajaxGet } from "../../../Framework/Signum.React/Scripts/Services";
import * as AuthClient from "../Authorization/AuthClient";

export function registerAuthenticator() {
    AuthClient.authenticators.push(loginFromApiKey);
}


export function start(options: { routes: JSX.Element[] }) {
    Navigator.addSettings(new EntitySettings(RestLogEntity, e => import('./Templates/RestLog')));
    Navigator.addSettings(new EntitySettings(RestApiKeyEntity, e => import('./Templates/RestApiKey')));
}

export function loginFromApiKey(): Promise<AuthClient.AuthenticatedUser | undefined> {
    const query = QueryString.parse(window.location.search);

    if ('apiKey' in query) {
        return API.loginFromApiKey(query.apiKey);
    }

    return Promise.resolve(undefined);
}

export module API {
    export function generateRestApiKey(): Promise<string> {
        return ajaxGet<string>({ url: "~/api/restApiKey" });
    }

    export function loginFromApiKey(apiKey: string): Promise<AuthClient.API.LoginResponse> {
        return ajaxGet<AuthClient.API.LoginResponse>({ url: "~/api/auth/loginFromApiKey?apiKey=" + apiKey, avoidAuthToken: true });
    }
}
