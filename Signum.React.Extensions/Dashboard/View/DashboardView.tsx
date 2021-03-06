﻿
import * as React from 'react'
import { FormGroup, FormControlStatic, ValueLine, ValueLineType, EntityLine, EntityCombo, EntityList, EntityRepeater, RenderEntity } from '../../../../Framework/Signum.React/Scripts/Lines'
import { classes } from '../../../../Framework/Signum.React/Scripts/Globals'
import * as Finder from '../../../../Framework/Signum.React/Scripts/Finder'
import { QueryDescription, SubTokensOptions } from '../../../../Framework/Signum.React/Scripts/FindOptions'
import { getQueryNiceName, PropertyRoute, getTypeInfos } from '../../../../Framework/Signum.React/Scripts/Reflection'
import { ModifiableEntity, EntityControlMessage, Entity, parseLite, getToString, Lite, toLite } from '../../../../Framework/Signum.React/Scripts/Signum.Entities'
import * as Navigator from '../../../../Framework/Signum.React/Scripts/Navigator'
import * as Constructor from '../../../../Framework/Signum.React/Scripts/Constructor'
import { TypeContext, FormGroupStyle, mlistItemContext } from '../../../../Framework/Signum.React/Scripts/TypeContext'
import QueryTokenEntityBuilder from '../../UserAssets/Templates/QueryTokenEntityBuilder'
import FileLine, { FileTypeSymbol } from '../../Files/FileLine'
import * as DashboardClient from '../DashboardClient'
import { DashboardEntity, PanelPartEmbedded, IPartEntity } from '../Signum.Entities.Dashboard'



require("../Dashboard.css");


export default class DashboardView extends React.Component<{ dashboard: DashboardEntity, entity?: Entity }, void> {

    render() {

        const db = this.props.dashboard;
        const entity = this.props.entity;

        const ctx = TypeContext.root(db);

        return (
            <div>
                {
                    mlistItemContext(ctx.subCtx(a => a.parts))
                        .groupBy(c => c.value.row!.toString())
                        .orderBy(gr => gr.key)
                        .map(gr =>
                            <div className="row row-control-panel" key={"row" + gr.key}>
                                {gr.elements.orderBy(ctx => ctx.value.startColumn).map((c, j, list) => {

                                    const last = j == 0 ? undefined : list[j - 1].value;

                                    const offset = c.value.startColumn! - (last ? (last.startColumn! + last.columns!) : 0);

                                    return (
                                        <div key={j} className={`col-sm-${c.value.columns} col-sm-offset-${offset}`}>
                                            <PanelPart ctx={c} entity={this.props.entity} />
                                        </div>
                                    );
                                })}
                            </div>)
                }
            </div>
        );
    }
}

export interface PanelPartProps {
    ctx: TypeContext<PanelPartEmbedded>;
    entity?: Entity;
}

export interface PanelPartState {
    component?: React.ComponentClass<DashboardClient.PanelPartContentProps<IPartEntity>>;
    lastType?: string;
}


export class PanelPart extends React.Component<PanelPartProps, PanelPartState>{

    state = { component: undefined, lastType: undefined } as PanelPartState;

    componentWillMount() {
        this.loadComponent(this.props);
    }

    componentWillReceiveProps(nextProps: PanelPartProps): void {

        if (this.state.lastType != nextProps.ctx.value.content!.Type) {
            this.loadComponent(nextProps);
        }
    }

    loadComponent(props: PanelPartProps) {
        const content = props.ctx.value.content!;
        this.setState({ component: undefined, lastType: undefined })
        DashboardClient.partRenderers[content.Type].component()
            .then(c => this.setState({ component: c, lastType: content.Type }))
            .done();
    }

    render() {

        if (!this.state.component)
            return null;

        const p = this.props.ctx.value;
        const content = p.content!;

        const renderer = DashboardClient.partRenderers[content.Type];


        const lite = this.props.entity ? toLite(this.props.entity) : undefined

        const title = p.title || getToString(content);

        return (
            <div className={classes("panel", "panel-" + (p.style == undefined ? "default" : p.style.firstLower()))}>
                <div className="panel-heading">
                    {renderer.handleTitleClick == undefined ? title : <a className="sf-pointer" onMouseUp={e => renderer.handleTitleClick!(content, lite, e)}>{title}</a>}
                    &nbsp;
                    {renderer.handleFullScreenClick &&
                        <a className="sf-ftbl-header-fullscreen sf-pointer" onMouseUp={e => renderer.handleFullScreenClick!(content, lite, e)}>
                            <span className="glyphicon glyphicon-new-window"></span>
                        </a>}
                </div>
                <div className="panel-body">
                    {
                        React.createElement(this.state.component, {
                            part: content,
                            entity: lite,
                        } as DashboardClient.PanelPartContentProps<IPartEntity>)
                    }
                </div>
            </div>
        );
    }
}



