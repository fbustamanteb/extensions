﻿
import * as React from 'react'
import { Dic } from '../../../../Framework/Signum.React/Scripts/Globals'
import { TypeContext, StyleOptions, EntityFrame  } from '../../../../Framework/Signum.React/Scripts/TypeContext'
import { TypeInfo, getTypeInfo, parseId, GraphExplorer, PropertyRoute, ReadonlyBinding, } from '../../../../Framework/Signum.React/Scripts/Reflection'
import * as Navigator from '../../../../Framework/Signum.React/Scripts/Navigator'
import * as Finder from '../../../../Framework/Signum.React/Scripts/Finder'
import * as Operations from '../../../../Framework/Signum.React/Scripts/Operations'
import { EntityPack, Entity, Lite, JavascriptMessage, entityInfo, getToString } from '../../../../Framework/Signum.React/Scripts/Signum.Entities'
import { renderWidgets, renderEmbeddedWidgets, WidgetContext } from '../../../../Framework/Signum.React/Scripts/Frames/Widgets'
import ValidationErrors from '../../../../Framework/Signum.React/Scripts/Frames/ValidationErrors'
import ButtonBar from '../../../../Framework/Signum.React/Scripts/Frames/ButtonBar'
import { CaseActivityEntity, WorkflowEntity, ICaseMainEntity, CaseActivityOperation, CaseActivityQuery, WorkflowMainEntityStrategy } from '../Signum.Entities.Workflow'
import * as WorkflowClient from '../WorkflowClient'

import { Navbar, Nav, NavItem, NavDropdown, MenuItem } from 'react-bootstrap'
import { LinkContainer, IndexLinkContainer } from 'react-router-bootstrap'

export default class WorkflowDropdown extends React.Component<{}, { starts: Array<WorkflowEntity> }>
{
    constructor(props: any) {
        super(props);
        this.state = { starts: [] };
    }

    componentWillMount() {
        WorkflowClient.API.starts()
            .then(starts => this.setState({ starts }))
            .done();
    }

    render() {

        const inboxUrl = WorkflowClient.getDefaultInboxUrl();
        
        return (
            <NavDropdown title={WorkflowEntity.nicePluralName()} id="workflow-dropdown" >
                <IndexLinkContainer to={inboxUrl}><MenuItem>{CaseActivityQuery.Inbox.niceName()}</MenuItem></IndexLinkContainer>
                {this.state.starts.length > 0 && <MenuItem divider />}
                {this.state.starts.length > 0 && <MenuItem disabled>{JavascriptMessage.create.niceToString()}</MenuItem>}
                {this.getStarts().map((val, i) =>
                    <LinkContainer key={i} to={`~/workflow/new/${val.workflow.id}/${val.mainEntityStrategy}`}>
                        <MenuItem>{val.workflow.toStr}{val.mainEntityStrategy == "SelectByUser" ? `(${WorkflowMainEntityStrategy.niceName(val.mainEntityStrategy)})` : ""}</MenuItem></LinkContainer>
                    )}
            </NavDropdown>
        );
    }

    getStarts() {
        return this.state.starts.flatMap((w, i) => {
            if (w.mainEntityStrategy != "Both")
                return [({ workflow: w, mainEntityStrategy: w.mainEntityStrategy! })]
            else
                return [({ workflow: w, mainEntityStrategy: ("CreateNew" as WorkflowMainEntityStrategy) })]
                    .concat([({ workflow: w, mainEntityStrategy: ("SelectByUser" as WorkflowMainEntityStrategy) })]);
        })
    }
}

