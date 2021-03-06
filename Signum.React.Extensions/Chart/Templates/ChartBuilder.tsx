﻿
import * as React from 'react'
import { DropdownButton, MenuItem, } from 'react-bootstrap'
import { Dic, classes } from '../../../../Framework/Signum.React/Scripts/Globals'
import * as Finder from '../../../../Framework/Signum.React/Scripts/Finder'
import { Lite, toLite } from '../../../../Framework/Signum.React/Scripts/Signum.Entities'
import { ResultTable, FindOptions, FilterOption, QueryDescription, SubTokensOptions, QueryToken, QueryTokenType } from '../../../../Framework/Signum.React/Scripts/FindOptions'
import { TypeContext, FormGroupSize, FormGroupStyle, StyleOptions, StyleContext, mlistItemContext } from '../../../../Framework/Signum.React/Scripts/TypeContext'
import { SearchMessage, JavascriptMessage, parseLite, is, liteKey } from '../../../../Framework/Signum.React/Scripts/Signum.Entities'
import * as Navigator from '../../../../Framework/Signum.React/Scripts/Navigator'
import { ValueLine, FormGroup, ValueLineProps, ValueLineType, OptionItem } from '../../../../Framework/Signum.React/Scripts/Lines'
import { ChartColumnEmbedded, ChartScriptColumnEmbedded, ChartScriptParameterEmbedded, IChartBase, GroupByChart, ChartMessage, ChartColorEntity, ChartScriptEntity, ChartParameterEmbedded, ChartParameterType } from '../Signum.Entities.Chart'
import * as ChartClient from '../ChartClient'
import QueryTokenEntityBuilder from '../../UserAssets/Templates/QueryTokenEntityBuilder'
import { ChartColumn, ChartColumnInfo }from './ChartColumn'

export interface ChartBuilderProps {
    ctx: TypeContext<IChartBase>; /*IChart*/
    queryKey: string;
    onInvalidate: () => void;
    onTokenChange: () => void;
    onRedraw: () => void;
}

export interface ChartBuilderState {
    chartScripts?: ChartScriptEntity[][],
    expanded?: boolean[];
    colorPalettes?: string[];
}


export default class ChartBuilder extends React.Component<ChartBuilderProps, ChartBuilderState> {

    constructor(props: ChartBuilderProps) {
        super(props);

        this.state = { expanded: undefined };
    }

    componentWillMount() {

        ChartClient.getChartScripts().then(scripts =>
            this.setState({ chartScripts: scripts }))
            .done();

        ChartClient.getColorPalettes().then(colorPalettes =>
            this.setState({ colorPalettes: colorPalettes }))
            .done();

        const ctx = this.props.ctx;

        ChartClient.synchronizeColumns(ctx.value);
        this.setState({ expanded: Array.repeat(ctx.value.columns.length, false) });
    }

    chartTypeImgClass(script: ChartScriptEntity): string {
        const cb = this.props.ctx.value;

        let css = "sf-chart-img";

        if (!cb.columns.some(a => a.element.token != undefined && a.element.token.parseException != undefined) && ChartClient.isCompatibleWith(script, cb))
            css += " sf-chart-img-equiv";

        if (is(cb.chartScript, script)){

            css += " sf-chart-img-curr";

            if(cb.chartScript!.script != script.script)
                css += " edited";
        }

        return css;
    }

    handleOnToggleInfo = (index: number) => {
        this.state.expanded![index] = !this.state.expanded![index];
        this.forceUpdate();
    }

    handleOnRedraw = () => {
        this.forceUpdate();
        this.props.onRedraw();
    }

    handleOnInvalidate = () => {
        this.forceUpdate();
        this.props.onInvalidate();
    }

    handleTokenChange = () => {
        this.forceUpdate();
        this.props.onTokenChange();
    }

    handleChartScriptOnClick = (cs: ChartScriptEntity) => {

        const chart = this.props.ctx.value;
        let compatible = ChartClient.isCompatibleWith(cs, chart)
        chart.chartScript = cs;
        ChartClient.synchronizeColumns(chart);

        if (!compatible)
            this.props.onInvalidate();
        else
            this.props.onRedraw();
    }

    render() {

        const chart = this.props.ctx.value;

        return (
            <div className="row sf-chart-builder">
                <div className="col-lg-2">
                    <div className="sf-chart-type panel panel-default">
                        <div className="panel-heading">
                            <h3 className="panel-title">{ChartScriptEntity.nicePluralName() }</h3>
                        </div>
                        <div className="panel-body">
                            {this.state.chartScripts && this.state.expanded && this.state.chartScripts.flatMap(a => a).map((cs, i) =>
                                <div key={i} className={this.chartTypeImgClass(cs) } title={cs.toStr + "\r\n" + cs.columnsStructure} onClick={() => this.handleChartScriptOnClick(cs)}>
                                    <img src={"data:image/jpeg;base64," + (cs.icon && cs.icon.entity && cs.icon.entity.binaryFile) }/>
                                </div>) }
                        </div>
                    </div>
                </div >
                <div className="col-lg-10">
                    <div className="sf-chart-tokens panel panel-default">
                        <div className="panel-heading">
                            <h3 className="panel-title">{ChartMessage.Chart_ChartSettings.niceToString() }</h3>
                        </div>
                        <div className="panel-body">
                            <table className="table" style={{ marginBottom: "0px" }}>
                                <thead>
                                    <tr>
                                        <th className="sf-chart-token-narrow">
                                            { ChartMessage.Chart_Dimension.niceToString() }
                                        </th>
                                        <th className="">
                                            { ChartMessage.Chart_Group.niceToString() }
                                        </th>
                                        <th className="sf-chart-token-wide">
                                            Token
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    { this.state.expanded && mlistItemContext(this.props.ctx.subCtx(c => c.columns, { formGroupSize: "ExtraSmall" })).flatMap((ctx, i) => [
                                        <ChartColumn chartBase={chart} ctx={ctx} key={"C" + i} scriptColumn={chart.chartScript!.columns[i].element} queryKey={this.props.queryKey}
                                            onToggleInfo={() => this.handleOnToggleInfo(i)} onGroupChange={this.handleOnInvalidate} onTokenChange={this.handleTokenChange} />,
                                        this.state.expanded![i] && this.state.colorPalettes && <ChartColumnInfo ctx= { ctx } key= { "CI" + i } colorPalettes= {this.state.colorPalettes} onRedraw= { this.handleOnRedraw } />
                                    ]) }
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <div className="sf-chart-parameters panel panel-default">
                        <div className="panel-body form-vertical">
                            {
                                this.state.expanded && mlistItemContext(this.props.ctx.subCtx(c => c.parameters, { formGroupStyle: "Basic", formGroupSize: "ExtraSmall" }))
                                    .map((ctx, i) => this.getParameterValueLine(ctx, chart.chartScript.parameters[i].element))
                                    .groupsOf(6).map((gr, j) =>
                                        <div className="row" key={j}>
                                            {gr.map((vl, i) => <div className="col-sm-2" key={i}>{vl}</div>) }
                                        </div>)
                            }
                        </div>
                    </div>
                </div>
            </div >);
    }



    getParameterValueLine(ctx: TypeContext<ChartParameterEmbedded>, scriptParameter: ChartScriptParameterEmbedded) {

        const chart = this.props.ctx.value;

        const vl: ValueLineProps = {
            ctx: ctx.subCtx(a => a.value, { labelColumns: { sm: 6 } }),
            labelText: scriptParameter.name!,
        };

        if (scriptParameter.type == "Number" || scriptParameter.type == "String") {
            vl.valueLineType = "TextBox";
        }
        else if (scriptParameter.type == "Enum") {
            vl.valueLineType = "ComboBox";

            const tokenEntity = scriptParameter.columnIndex == undefined ? undefined : chart.columns[scriptParameter.columnIndex].element.token;

            const compatible = scriptParameter.enumValues.filter(a => a.typeFilter == undefined || tokenEntity == undefined || ChartClient.isChartColumnType(tokenEntity.token, a.typeFilter));
            if (compatible.length <= 1)
                vl.ctx.styleOptions.readOnly = true;

            vl.comboBoxItems = compatible.map(ev => ({
                value: ev.name,
                label: ev.name
            } as OptionItem));

            vl.valueHtmlAttributes = { size: null as any };
        }

        vl.onChange = this.handleOnRedraw;

        return <ValueLine {...vl} />;
    }

}
