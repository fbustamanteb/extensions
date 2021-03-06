﻿import * as React from 'react'
import { Dic } from '../../../../Framework/Signum.React/Scripts/Globals'
import { getMixin } from '../../../../Framework/Signum.React/Scripts/Signum.Entities'
import { ColorTypeaheadLine } from '../../Basics/Templates/ColorTypeahead'
import { CaseTagTypeEntity } from '../Signum.Entities.Workflow'
import {
    ValueLine, EntityLine, RenderEntity, EntityCombo, EntityList, EntityDetail, EntityStrip,
    EntityRepeater, EntityCheckboxList, EntityTabRepeater, TypeContext, EntityTable
} from '../../../../Framework/Signum.React/Scripts/Lines'
import { SearchControl, ValueSearchControl } from '../../../../Framework/Signum.React/Scripts/Search'
import Tag from './Tag'

export default class CaseTagTypeComponent extends React.Component < { ctx: TypeContext<CaseTagTypeEntity> }, void> {

    render() {
        var ctx = this.props.ctx;
        return (
            <div className="row">
                <div className="col-sm-10">
                    <ValueLine ctx={ctx.subCtx(e => e.name)} onChange={() => this.forceUpdate()} />
                    <ColorTypeaheadLine ctx={ctx.subCtx(e => e.color)} onChange={() => this.forceUpdate()} />
                </div>
                <div className="col-sm-2">
                    <Tag tag={this.props.ctx.value} />
                </div>
            </div>
        );
    }
}