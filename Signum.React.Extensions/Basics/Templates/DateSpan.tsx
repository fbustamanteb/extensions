﻿import * as React from 'react'
import { Tab, Tabs } from 'react-bootstrap'
import { classes } from '../../../../Framework/Signum.React/Scripts/Globals'
import { FormGroup, FormControlStatic, ValueLine, ValueLineType, EntityLine, EntityCombo, EntityDetail, EntityList, EntityRepeater, EntityTabRepeater } from '../../../../Framework/Signum.React/Scripts/Lines'
import { SubTokensOptions, QueryToken, QueryTokenType, hasAnyOrAll } from '../../../../Framework/Signum.React/Scripts/FindOptions'
import { SearchControl } from '../../../../Framework/Signum.React/Scripts/Search'
import { getToString, getMixin } from '../../../../Framework/Signum.React/Scripts/Signum.Entities'
import { TypeContext, FormGroupStyle } from '../../../../Framework/Signum.React/Scripts/TypeContext'
import { DateSpanEmbedded } from '../Signum.Entities.Basics'
import { TemplateTokenMessage } from '../../Templating/Signum.Entities.Templating'
import FileLine from '../../Files/FileLine'
import QueryTokenEntityBuilder from '../../UserAssets/Templates/QueryTokenEntityBuilder'
import TemplateControls from '../../Templating/TemplateControls'
import ValueLineModal from '../../../../Framework/Signum.React/Scripts/ValueLineModal'

export default class DateSpan extends React.Component<{ ctx: TypeContext<DateSpanEmbedded> }, void> {

    render() {

        const e = this.props.ctx;
        const sc = e.subCtx({ formGroupStyle: "BasicDown" });

        return (
            <div className="row form-vertical">
                <div className="col-sm-4">
                    <ValueLine ctx={sc.subCtx(n => n.years)} />
                </div>
                <div className="col-sm-4">
                    <ValueLine ctx={sc.subCtx(n => n.months)} />
                </div>
                <div className="col-sm-4">
                    <ValueLine ctx={sc.subCtx(n => n.days)} />
                </div>
            </div>
        );
    }
}
