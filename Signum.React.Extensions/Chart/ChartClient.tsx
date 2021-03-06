import * as React from 'react'
import { Route } from 'react-router'
import * as QueryString from 'query-string'
import { Dic } from '../../../Framework/Signum.React/Scripts/Globals';
import { ajaxPost, ajaxGet } from '../../../Framework/Signum.React/Scripts/Services';
import { EntitySettings, ViewPromise } from '../../../Framework/Signum.React/Scripts/Navigator'
import * as Navigator from '../../../Framework/Signum.React/Scripts/Navigator'
import * as Finder from '../../../Framework/Signum.React/Scripts/Finder'
import { EntityOperationSettings } from '../../../Framework/Signum.React/Scripts/Operations'
import { Entity, Lite, liteKey, MList } from '../../../Framework/Signum.React/Scripts/Signum.Entities'
import * as Constructor from '../../../Framework/Signum.React/Scripts/Constructor'
import * as Operations from '../../../Framework/Signum.React/Scripts/Operations'
import * as QuickLinks from '../../../Framework/Signum.React/Scripts/QuickLinks'
import { PseudoType, QueryKey, getQueryKey } from '../../../Framework/Signum.React/Scripts/Reflection'
import {
    FindOptions, FilterOption, FilterOptionParsed, FilterOperation, OrderOption, OrderOptionParsed, ColumnOption,
    FilterRequest, QueryRequest, Pagination, QueryTokenType, QueryToken, FilterType, SubTokensOptions, ResultTable, OrderRequest } from '../../../Framework/Signum.React/Scripts/FindOptions'
import * as AuthClient  from '../../../Extensions/Signum.React.Extensions/Authorization/AuthClient'
import { QueryFilterEmbedded, QueryColumnEmbedded, QueryOrderEmbedded } from '../UserQueries/Signum.Entities.UserQueries'

import {
    UserChartEntity, ChartPermission, ChartMessage, ChartColumnEmbedded, ChartParameterEmbedded, ChartScriptEntity, ChartScriptParameterEmbedded, ChartRequest,
    GroupByChart, ChartColumnType, IChartBase } from './Signum.Entities.Chart'
import { QueryTokenEmbedded } from '../UserAssets/Signum.Entities.UserAssets'
import ChartButton from './ChartButton'
import ChartRequestView from './Templates/ChartRequestView'
import * as UserChartClient from './UserChart/UserChartClient'
import { ImportRoute } from "../../../Framework/Signum.React/Scripts/AsyncImport";

export function start(options: { routes: JSX.Element[] }) {

    options.routes.push(<ImportRoute path="~/chart/:queryName" onImportModule={() => import("./Templates/ChartRequestPage")} />);

    Finder.ButtonBarQuery.onButtonBarElements.push(ctx => {
        if (!ctx.searchControl.props.showBarExtension || !AuthClient.isPermissionAuthorized(ChartPermission.ViewCharting))
            return undefined;

        return <ChartButton searchControl={ctx.searchControl}/>;
    });

    Navigator.addSettings(new EntitySettings(ChartScriptEntity, e => import('./ChartScript/ChartScript')));

    UserChartClient.start({ routes: options.routes });
}


export namespace ButtonBarChart {

    interface ButtonBarChartContext {
        chartRequestView: ChartRequestView;
        chartRequest: ChartRequest;
    }

    export const onButtonBarElements: ((ctx: ButtonBarChartContext) => React.ReactElement<any> | undefined)[] = [];

    export function getButtonBarElements(ctx: ButtonBarChartContext): React.ReactElement<any>[] {
        return onButtonBarElements.map(f => f(ctx)).filter(a => a != undefined).map(a => a!);
    }
}

export let chartScripts: ChartScriptEntity[][];
export function getChartScripts(): Promise<ChartScriptEntity[][]> {
    if (chartScripts)
        return Promise.resolve(chartScripts);

    return API.fetchScripts().then(cs => chartScripts = cs);
}

export let colorPalettes: string[];
export function getColorPalettes(): Promise<string[]> {
    if (colorPalettes)
        return Promise.resolve(colorPalettes);

    return API.fetchColorPalettes().then(cs => colorPalettes = cs);
}



export function isCompatibleWith(chartScript: ChartScriptEntity, chartBase: IChartBase): boolean {
    if (chartScript.groupBy == "Always" && !chartBase.groupResults)
        return false;

    if (chartScript.groupBy == "Never" && chartBase.groupResults)
        return false;

    return zipOrDefault(
        chartScript.columns!.map(mle => mle.element),
        chartBase.columns!.map(mle => mle.element), (s, c) => {

            if (s == undefined)
                return c!.token == undefined;

            if (c == undefined || c.token == undefined)
                return s.isOptional!;

            if (!isChartColumnType(c.token.token, s.columnType!))
                return false;

            if (c.token.token!.queryTokenType == "Aggregate")
                return !s.isGroupKey;
            else
                return s.isGroupKey || !chartBase.groupResults;
        }).every(b => b);
}

export function zipOrDefault<T, S, R>(arrayT: T[], arrayS: S[], selector: (t: T | undefined, s: S | undefined) => R): R[] {
    const max = Math.max(arrayT.length, arrayS.length);

    const result: R[] = [];
    for (let i = 0; i < max; i++) {
        result.push(selector(
            i < arrayT.length ? arrayT[i] : undefined,
            i < arrayS.length ? arrayS[i] : undefined));
    }

    return result;
}

export function isChartColumnType(token: QueryToken | undefined, ct: ChartColumnType): boolean {
    if (token == undefined)
        return false;

    const type = getChartColumnType(token);

    if (type == undefined)
        return false;

    if (ct == type)
        return true;


    switch (ct) {

        case "Groupable": return [
            "RealGroupable",
            "Integer",
            "Date",
            "String",
            "Lite",
            "Enum"].contains(type);

        case "Magnitude": return [
            "Integer",
            "Real",
            "RealGroupable"].contains(type);

        case "Positionable": return [
            "Integer",
            "Real",
            "RealGroupable",
            "Date",
            "DateTime",
            "Enum"].contains(type);
    }


    return false;
}

export function getChartColumnType(token: QueryToken): ChartColumnType | undefined {

    switch (token.filterType) {
        case "Lite": return "Lite";
        case "Boolean":
        case "Enum": return "Enum";
        case "String":
        case "Guid": return "String";
        case "Integer": return "Integer";
        case "Decimal": return token.isGroupable ? "RealGroupable" : "Real";
        case "DateTime": return token.isGroupable ? "Date" : "DateTime";
    }

    return undefined;
}




export function synchronizeColumns(chart: IChartBase) {

    const chartScript = chart.chartScript!;

    if (chart.columns == null ||
        chartScript.columns == null ||
        chartScript.parameters == null ||
        chart.parameters == null)
        throw Error("no Columns");

    if (chartScript == undefined) {
        chart.columns!.clear();
    }

    for (let i = 0; i < chartScript.columns!.length; i++) {
        if (chart.columns.length <= i) {
            chart.columns.push({ rowId: null, element: ChartColumnEmbedded.New() });
        }
    }

    if (chart.columns.length > chartScript.columns.length) {
        chart.columns.splice(chartScript.columns.length, chart.columns.length - chartScript.columns.length);
    }


    if (chart.parameters.map(a => a.element.name!).orderBy(n => n).join(" ") !=
        chartScript.parameters.map(a => a.element.name!).orderBy(n => n).join(" ")) {

        const byName = chart.parameters.map(a => a.element).toObject(a => a.name!);
        chart.parameters.clear();

        chartScript.parameters.forEach(sp => {
            let cp = byName[sp.element.name!];

            if (cp == undefined) {
                cp = ChartParameterEmbedded.New();
                cp.name = sp.element.name;
                const column = sp.element.columnIndex == undefined ? undefined : chart.columns![sp.element.columnIndex].element;
                cp.value = defaultParameterValue(sp.element, column && column.token && column.token.token);
            }
            else {
                const column = sp.element.columnIndex == undefined ? undefined : chart.columns![sp.element.columnIndex].element;
                if (isValidParameterValue(cp.value, sp.element, column && column.token && column.token.token))
                    defaultParameterValue(sp.element, column && column.token && column.token.token);
            }

            chart.parameters!.push({ rowId: null, element: cp });
        });
    }

    if (chart.groupResults == undefined) {
        chart.groupResults = true;
    }

    if (chartScript.groupBy == "Always" && chart.groupResults == false) {
        chart.groupResults = true;
    }
    else if (chartScript.groupBy == "Never" && chart.groupResults == true) {
        chart.groupResults = false;
    }

    chart.columns.map(mle => mle.element).forEach((cc, i) => {
        if (cc.token && cc.token.token!.queryTokenType == "Aggregate") {

            const sc = chart.chartScript!.columns![i]
            if (chart.groupResults == false || sc && sc.element.isGroupKey) {
                const parentToken = cc.token.token!.parent;
                cc.token = parentToken == undefined ? undefined : QueryTokenEmbedded.New({
                    tokenString : parentToken && parentToken.fullKey,
                    token : parentToken
                });
            }
        }
    });

    if (chart.Type == ChartRequest.typeName) {
        const cr = chart as ChartRequest;

        const keys = chart.columns.filter((a, i) => a.element.token && chartScript.columns![i].element.isGroupKey).map(a => a.element.token!.token!.fullKey);

        cr.orderOptions = cr.orderOptions!.filter(o => {
            if (chart.groupResults)
                return o.token!.queryTokenType == "Aggregate" || keys.contains(o.token!.fullKey);
            else
                return o.token!.queryTokenType != "Aggregate";
        });
    }
}

function isValidParameterValue(value: string | null | undefined, scrptParameter: ChartScriptParameterEmbedded, relatedColumn: QueryToken | null | undefined) {

    switch (scrptParameter.type) {
        case "Enum": return scrptParameter.enumValues.filter(a => a.typeFilter == undefined || relatedColumn == undefined || isChartColumnType(relatedColumn, a.typeFilter)).some(a => a.name == value);
        case "Number": return !isNaN(parseFloat(value!));
        case "String": return true;
        default: throw new Error("Unexpected parameter type");
    }

}

function defaultParameterValue(scriptParameter: ChartScriptParameterEmbedded, relatedColumn: QueryToken | null |  undefined) {

    switch (scriptParameter.type) {
        case "Enum": return scriptParameter.enumValues.filter(a => a.typeFilter == undefined || relatedColumn == undefined || isChartColumnType(relatedColumn, a.typeFilter)).first().name;
        case "Number": return parseFloat(scriptParameter.valueDefinition!).toString();
        case "String": return scriptParameter.valueDefinition;
        default: throw new Error("Unexpected parameter type");
    }

}


export module Encoder {

    export function chartRequestPath(cr: ChartRequest, extra?: any): string {
        const query = {
            script: cr.chartScript && cr.chartScript.name,
            groupResults: cr.groupResults,
            ...extra
        };

        Finder.Encoder.encodeFilters(query, cr.filterOptions.map(a => ({ columnName: a.token!.fullKey, operation: a.operation, value: a.value, frozen: a.frozen } as FilterOption)));
        Finder.Encoder.encodeOrders(query, cr.orderOptions.map(a => ({ columnName: a.token!.fullKey, orderType: a.orderType } as OrderOption)));
        encodeParameters(query, cr.parameters);

        encodeColumn(query, cr.columns);

        return Navigator.toAbsoluteUrl(`~/chart/${cr.queryKey}?` + QueryString.stringify(query));
    }

    const scapeTilde = Finder.Encoder.scapeTilde;

    export function encodeColumn(query: any, columns: MList<ChartColumnEmbedded>) {
        if (columns)
            columns.forEach((co, i) => query["column" + i] = (co.element.token ? co.element.token.tokenString : "") + (co.element.displayName ? ("~" + scapeTilde(co.element.displayName)) : ""));
    }
    export function encodeParameters(query: any, parameters: MList<ChartParameterEmbedded>) {
        if (parameters)
            parameters.map((p, i) => query["param" + i] = scapeTilde(p.element.name!) + "~" + scapeTilde(p.element.value!));
    }
}


export module Decoder {

    export function parseChartRequest(queryName: string, query: any): Promise<ChartRequest> {
        
        return getChartScripts().then(scripts => {
            return Finder.getQueryDescription(queryName).then(qd => {

                const completer = new Finder.TokenCompleter(qd);

                const fos = Finder.Decoder.decodeFilters(query);
                fos.forEach(fo => completer.request(fo.columnName, SubTokensOptions.CanElement | SubTokensOptions.CanAnyAll | SubTokensOptions.CanAggregate));

                const oos = Finder.Decoder.decodeOrders(query);
                oos.forEach(oo => completer.request(oo.columnName, SubTokensOptions.CanElement | SubTokensOptions.CanAggregate));

                const cols = Decoder.decodeColumns(query);
                cols.map(a => a.element.token).filter(te => te != undefined).forEach(te =>completer.request(te!.tokenString!, SubTokensOptions.CanAggregate | SubTokensOptions.CanElement));

                return completer.finished().then(() => {

                    cols.filter(a => a.element.token != null).forEach(a => a.element.token!.token = completer.get(a.element.token!.tokenString));

                    const chartRequest = ChartRequest.New({
                        chartScript : query.script == undefined ? scripts.first("ChartScript").first() :
                            scripts.flatMap(a => a).filter(cs => cs.name == query.script).single(`ChartScript '${query.queryKey}'`),
                        queryKey : getQueryKey(queryName),
                        groupResults : query.groupResults == "true",
                        filterOptions : fos.map(fo => ({ token: completer.get(fo.columnName), operation: fo.operation, value: fo.value, frozen: fo.frozen }) as FilterOptionParsed),
                        orderOptions : oos.map(oo => ({ token: completer.get(oo.columnName), orderType: oo.orderType }) as OrderOptionParsed),
                        columns : cols,
                        parameters : Decoder.decodeParameters(query),
                    });

                    return Finder.parseFilterValues(chartRequest.filterOptions)
                        .then(() => chartRequest);
                });
            });
        });
    }


    const unscapeTildes = Finder.Decoder.unscapeTildes;
    const valuesInOrder = Finder.Decoder.valuesInOrder;

    export function decodeColumns(query: any): MList<ChartColumnEmbedded> {
        return valuesInOrder(query, "column").map(val => {
            const ts = (val.contains("~") ? val.before("~") : val).trim();

            return ({
                rowId: null,
                element: ChartColumnEmbedded.New({
                    token: !!ts ? QueryTokenEmbedded.New({
                        tokenString: ts,
                    }) : undefined,
                    displayName: unscapeTildes(val.tryAfter("~")),
                })
            });
        });
    }

    export function decodeParameters(query: any): MList<ChartParameterEmbedded> {
        return valuesInOrder(query, "param").map(val => ({
            rowId: null,
            element: ChartParameterEmbedded.New({
                name : unscapeTildes(val.before("~")),
                value : unscapeTildes(val.after("~")),
            })
        }));
    }
}


export module API {



    export function cleanedChartRequest(request: ChartRequest) {
        const clone = {...request };

        clone.orders = clone.orderOptions!.map(oo => ({ token: oo.token.fullKey, orderType: oo.orderType }) as OrderRequest);
        delete clone.orderOptions;

        clone.filters = clone.filterOptions!.filter(a => a.token != null).map(fo => ({ token: fo.token!.fullKey, operation: fo.operation, value: fo.value }) as FilterRequest);
        delete clone.filterOptions;

        return clone;
    }

    export function executeChart(request: ChartRequest, abortController?: FetchAbortController): Promise<ExecuteChartResult> {

        const clone = cleanedChartRequest(request);

        return ajaxPost<ExecuteChartResult>({ url: "~/api/chart/execute", abortController }, clone);

    }
    export interface ExecuteChartResult {
        resultTable: ResultTable;
        chartTable: ChartTable;
    }

    export function fetchScripts(): Promise<ChartScriptEntity[][]> {
        return ajaxGet<ChartScriptEntity[][]>({
            url: "~/api/chart/scripts"
        });
    }

    export function fetchColorPalettes(): Promise<string[]> {
        return ajaxGet<string[]>({
            url: "~/api/chart/colorPalettes"
        });
    }
}


export interface ChartValue {
    key: string | number | undefined,
    toStr: string,
    color: string,
    niceToString(): string;
}

export interface ChartTable {
    columns: { [name: string]: ChartColumn },
    parameters: { [name: string]: string | null | undefined },
    rows: ChartRow[]
}

export interface ChartRow {
    [name: string]: ChartValue
}


export interface ChartColumn {
    title?: string;
    displayName?: string;
    token?: string;
    isGroupKey?: boolean;
    type?: string;
}

