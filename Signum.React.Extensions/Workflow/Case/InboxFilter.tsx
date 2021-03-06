﻿import * as React from 'react'
import * as moment from 'moment'
import { Button } from "react-bootstrap"
import { Binding, LambdaMemberType } from '../../../../Framework/Signum.React/Scripts/Reflection'
import { Dic } from '../../../../Framework/Signum.React/Scripts/Globals'
import { newMListElement } from '../../../../Framework/Signum.React/Scripts/Signum.Entities'
import { InboxFilterModel, InboxMessage, CaseNotificationState } from '../Signum.Entities.Workflow'
import { TypeContext, ValueLine, EntityLine, EntityCombo, EntityList, EntityDetail, EntityStrip, EntityRepeater, EnumCheckboxList, FormGroup, FormGroupStyle, FormGroupSize, StyleContext } from '../../../../Framework/Signum.React/Scripts/Lines'
import { SearchControl, ValueSearchControl, FilterOperation, OrderType, PaginationMode, ISimpleFilterBuilder, extractFilterValue, FilterOption, FindOptionsParsed } from '../../../../Framework/Signum.React/Scripts/Search'
import { FilterOptionParsed } from "../../../../Framework/Signum.React/Scripts/FindOptions";

export default class InboxFilter extends React.Component<{ ctx: TypeContext<InboxFilterModel> }, void> implements ISimpleFilterBuilder {

    handleOnClearFiltersClick = () => {
        //this.props.ctx.value = CaseNotificationFilterModel.New();
        InboxFilter.resetModel(this.props.ctx.value);
        this.forceUpdate();
    };

    static resetModel(model: InboxFilterModel) {
        model.range = "All";
        model.states = [
            newMListElement("New" as CaseNotificationState),
            newMListElement("Opened" as CaseNotificationState),
            newMListElement("InProgress" as CaseNotificationState)
        ];
        model.fromDate = null;
        model.toDate = null;
    }

    render() {
        var ctx = this.props.ctx;
        return (
            <div className="form-vertical" style={{ marginBottom: 15 }}>
                <div className="row">
                    <div className="col-sm-2">
                        <EnumCheckboxList ctx={ctx.subCtx(o => o.states)} />
                    </div>
                    <div className="col-sm-4">
                        <ValueLine ctx={ctx.subCtx(o => o.range)} />
                        <ValueLine ctx={ctx.subCtx(o => o.fromDate)} />
                        <ValueLine ctx={ctx.subCtx(o => o.toDate)} />
                        <Button bsStyle="warning" className="btn btn-sm" style={{ marginTop: 5, marginLeft: 15 }} onClick={this.handleOnClearFiltersClick} > {InboxMessage.Clear.niceToString()} </Button>
                    </div>
                </div>
            </div>);
    }

    getFilters(): FilterOption[] {

        var result: FilterOption[] = [];

        var val = this.props.ctx.value;

        if (val.range) {
            var fromDate: string | undefined;
            var toDate: string | undefined;
            var startOfYear: Date;
            var isPersian = moment.locale() == "fa";
            var monthUnit = isPersian ? "jMonth" : "month";
            var yearUnit = isPersian ? "jYear" : "year";
            var now: Date = moment(Date.now()).toDate();

            startOfYear = moment(now).startOf(yearUnit as any).toDate();
            switch (val.range) {
                case "All":
                    break;

                case "LastWeek":
                    fromDate = moment(now).add(-7, "day").toDate().toISOString();
                    break;

                case "LastMonth":
                    fromDate = moment(now).add(-30, "day").toDate().toISOString();
                    break;

                case "CurrentYear":
                    {
                        fromDate = startOfYear.toISOString();
                        break;
                    }
            }

            if (fromDate && fromDate.length > 0)
                result.push({ columnName: "StartDate", operation: "GreaterThanOrEqual", value: fromDate });

            if (toDate && toDate.length > 0)
                result.push({ columnName: "StartDate", operation: "LessThanOrEqual", value: toDate });
        }

        if (val.states)
            result.push({ columnName: "State", operation: "IsIn", value: val.states.map(elm => elm.element) });

        if (val.fromDate)
            result.push({ columnName: "StartDate", value: val.fromDate, operation: "GreaterThanOrEqual" });

        if (val.toDate)
            result.push({ columnName: "StartDate", value: val.toDate, operation: "LessThanOrEqual" });

        return result;
    }

    static extract(fos: FilterOptionParsed[]): InboxFilterModel | null {
        var filters = fos.clone();

        var result = InboxFilterModel.New({
            range: extractFilterValue(filters, "Range", "EqualTo"),
            states: (extractFilterValue(filters, "State", "IsIn") as CaseNotificationState[] || []).map(b => newMListElement(b)),
            fromDate: extractFilterValue(filters, "StartDate", "GreaterThanOrEqual"),
            toDate: extractFilterValue(filters, "StartDate", "LessThanOrEqual"),
        });

        if (filters.length)
            return null;

        return result;
    }
}