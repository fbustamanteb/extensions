﻿import * as React from 'react'
import { Modal, ModalProps, ModalClass, ButtonToolbar, Button } from 'react-bootstrap'
import { FormGroup, FormControlStatic, ValueLine, ValueLineType, EntityLine, EntityCombo, EntityList, EntityRepeater } from '../../../../Framework/Signum.React/Scripts/Lines'
import { classes, Dic } from '../../../../Framework/Signum.React/Scripts/Globals'
import * as Finder from '../../../../Framework/Signum.React/Scripts/Finder'
import { QueryDescription, SubTokensOptions, QueryToken, filterOperations, OrderType, ColumnOptionsMode } from '../../../../Framework/Signum.React/Scripts/FindOptions'
import { getQueryNiceName, getTypeInfo, isTypeEntity, Binding } from '../../../../Framework/Signum.React/Scripts/Reflection'
import * as Navigator from '../../../../Framework/Signum.React/Scripts/Navigator'
import { TypeContext, FormGroupStyle } from '../../../../Framework/Signum.React/Scripts/TypeContext'
import QueryTokenBuilder from '../../../../Framework/Signum.React/Scripts/SearchControl/QueryTokenBuilder'
import { ModifiableEntity, JavascriptMessage, EntityControlMessage } from '../../../../Framework/Signum.React/Scripts/Signum.Entities'
import { QueryEntity } from '../../../../Framework/Signum.React/Scripts/Signum.Entities.Basics'
import { FilterOperation, PaginationMode } from '../../../../Framework/Signum.React/Scripts/Signum.Entities.DynamicQuery'
import { ExpressionOrValueComponent, FieldComponent, DesignerModal } from './Designer'
import * as Nodes from './Nodes'
import * as NodeUtils from './NodeUtils'
import { DesignerNode, Expression, ExpressionOrValue, isExpression } from './NodeUtils'
import { FindOptionsComponent } from './FindOptionsComponent'
import { BaseNode } from './Nodes'
import { HtmlAttributesExpression } from './HtmlAttributesExpression'
import { openModal, IModalProps } from '../../../../Framework/Signum.React/Scripts/Modals';
import SelectorModal from '../../../../Framework/Signum.React/Scripts/SelectorModal';
import { DynamicViewMessage, DynamicViewValidationMessage } from '../Signum.Entities.Dynamic'
import * as DynamicViewClient from '../DynamicViewClient'
import Typeahead from '../../../../Framework/Signum.React/Scripts/Lines/Typeahead'

interface HtmlAttributesLineProps {
    binding: Binding<HtmlAttributesExpression | undefined>;
    dn: DesignerNode<BaseNode>;
}

export class HtmlAttributesLine extends React.Component<HtmlAttributesLineProps, void>{

    renderMember(expr: HtmlAttributesExpression | undefined): React.ReactNode {
        return (<span
            className={expr === undefined ? "design-default" : "design-changed"}>
            {this.props.binding.member}
        </span>);
    }

    handleRemove = () => {
        this.props.binding.deleteValue();
        this.props.dn.context.refreshView();
    }

    handleCreate = () => {
        this.modifyExpression({} as HtmlAttributesExpression);
    }

    handleView = (e: React.MouseEvent<any>) => {
        e.preventDefault();
        var hae = JSON.parse(JSON.stringify(this.props.binding.getValue())) as HtmlAttributesExpression;
        this.modifyExpression(hae);
    }

    modifyExpression(hae: HtmlAttributesExpression) {

        if (hae.style == undefined)
            hae.style = {};

        DesignerModal.show("HtmlAttributes", () => <HtmlExpressionComponent dn={this.props.dn} htmlAttributes={hae} />).then(result => {
            if (result) {
                if ((hae as Object).hasOwnProperty("style") && Dic.getKeys(hae.style!).length == 0)
                    delete hae.style;

                if (Dic.getKeys(hae).length == 0)
                    this.props.binding.deleteValue();
                else
                    this.props.binding.setValue(hae);
            }

            this.props.dn.context.refreshView();
        }).done();
    }

    render() {
        const val = this.props.binding.getValue();

        return (
            <div className="form-group">
                <label className="control-label">
                    {this.renderMember(val)}

                    {val && " "}
                    {val && <a className={classes("sf-line-button", "sf-remove")}
                        onClick={this.handleRemove}
                        title={EntityControlMessage.Remove.niceToString()}>
                        <span className="glyphicon glyphicon-remove" />
                    </a>}
                </label>
                <div>
                    {val ?
                        <a href="" onClick={this.handleView}><pre style={{ padding: "0px", border: "none" }}>{this.getDescription(val)}</pre></a>
                        :
                        <a title={EntityControlMessage.Create.niceToString()}
                            className="sf-line-button sf-create"
                            onClick={this.handleCreate}>
                            <span className="glyphicon glyphicon-plus sf-create sf-create-label" />{EntityControlMessage.Create.niceToString()}
                        </a>}
                </div>
            </div>
        );
    }
    getDescription(hae: HtmlAttributesExpression) {

        var { style, ...cleanHae } = hae;


        var keys = Dic.map(cleanHae, (key, value) => key + ":" + this.getValue(value));

        if (style)
            keys.push("style: {\n" +
                Dic.map(style, (key, value) => "   " + key + ":" + this.getValue(value)).join("\n") +
                "\n}");

        return keys.join("\n");
    }

    getValue(value : any) {
        return (isExpression(value) ? "{" + value.__code__ + "}" : value)
    }
}

export interface HtmlExpressionComponentProps {
    dn: DesignerNode<BaseNode>;
    htmlAttributes: HtmlAttributesExpression
}

const htmlAttributeList = ["accept", "accept-charset", "accesskey", "action", "align", "alt", "async", "autocomplete", "autofocus", "autoplay", "autosave", "bgcolor", "border",
    "buffered", "challenge", "charset", "checked", "cite", "class", "code", "codebase", "color", "cols", "colspan", "content", "contenteditable", "contextmenu", "controls",
    "coords", "data", "data-*", "datetime", "default", "defer", "dir", "dirname", "disabled", "download", "draggable", "dropzone", "enctype", "for", "form", "formaction",
    "headers", "height", "hidden", "high", "href", "hreflang", "http-equiv", "icon", "id", "ismap", "itemprop", "keytype", "kind", "label", "lang", "language", "list",
    "loop", "low", "manifest", "max", "maxlength", "media", "method", "min", "multiple", "muted", "name", "novalidate", "open", "optimum", "pattern", "ping", "placeholder",
    "poster", "preload", "radiogroup", "readonly", "rel", "required", "reversed", "rows", "rowspan", "sandbox", "scope", "scoped", "seamless", "selected", "shape", "size",
    "sizes", "span", "spellcheck", "src", "srcdoc", "srclang", "srcset", "start", "step", "style", "summary", "tabindex", "target", "title", "type", "usemap", "value", "width",
    "wrap"].sort();

const cssPropertyList = ["color", "opacity", "background", "background-attachment", "background-blend-mode", "background-color", "background-image", "background-position",
    "background-repeat", "background-clip", "background-origin", "background-size", "border", "border-bottom", "border-bottom-color", "border-bottom-left-radius",
    "border-bottom-right-radius", "border-bottom-style", "border-bottom-width", "border-color", "border-image", "border-image-outset", "border-image-repeat",
    "border-image-slice", "border-image-source", "border-image-width", "border-left", "border-left-color", "border-left-style", "border-left-width", "border-radius",
    "border-right", "border-right-color", "border-right-style", "border-right-width", "border-style", "border-top", "border-top-color", "border-top-left-radius",
    "border-top-right-radius", "border-top-style", "border-top-width", "border-width", "box-decoration-break", "box-shadow", "bottom", "clear", "clip", "display", "float",
    "height", "left", "margin", "margin-bottom", "margin-left", "margin-right", "margin-top", "max-height", "max-width", "min-height", "min-width", "overflow",
    "overflow-x", "overflow-y", "padding", "padding-bottom", "padding-left", "padding-right", "padding-top", "position", "right", "top", "visibility", "width", "vertical-align",
    "z-index", "align-content", "align-items", "align-self", "flex", "flex-basis", "flex-direction", "flex-flow", "flex-grow", "flex-shrink", "flex-wrap", "justify-content",
    "order", "hanging-punctuation", "hyphens", "letter-spacing", "line-break", "line-height", "overflow-wrap", "tab-size", "text-align", "text-align-last", "text-combine-upright",
    "text-indent", "text-justify", "text-transform", "white-space", "word-break", "word-spacing", "word-wrap", "text-decoration", "text-decoration-color", "text-decoration-line",
    "text-decoration-style", "text-shadow", "text-underline-position", "@font-face", "@font-feature-values", "font", "font-family", "font-feature-settings", "font-kerning",
    "font-language-override", "font-size", "font-size-adjust", "font-stretch", "font-style", "font-synthesis", "font-variant", "font-variant-alternates", "font-variant-caps",
    "font-variant-east-asian", "font-variant-ligatures", "font-variant-numeric", "font-variant-position", "font-weight", "direction", "text-orientation", "text-combine-upright",
    "unicode-bidi", "writing-mode", "border-collapse", "border-spacing", "caption-side", "empty-cells", "table-layout", "counter-increment", "counter-reset", "list-style",
    "list-style-image", "list-style-position", "list-style-type", "@keyframes", "animation", "animation-delay", "animation-direction", "animation-duration", "animation-fill-mode",
    "animation-iteration-count", "animation-name", "animation-play-state", "animation-timing-function", "backface-visibility", "perspective", "perspective-origin", "transform",
    "transform-origin", "transform-style", "transition", "transition-property", "transition-duration", "transition-timing-function", "transition-delay", "box-sizing", "content",
    "cursor", "ime-mode", "nav-down", "nav-index", "nav-left", "nav-right", "nav-up", "outline", "outline-color", "outline-offset", "outline-style", "outline-width", "resize",
    "text-overflow", "break-after", "break-before", "break-inside", "column-count", "column-fill", "column-gap", "column-rule", "column-rule-color", "column-rule-style",
    "column-rule-width", "column-span", "column-width", "columns", "widows", "orphans", "page-break-after", "page-break-before", "page-break-inside", "marks", "quotes", "filter",
    "image-orientation", "image-rendering", "image-resolution", "object-fit", "object-position", "mask", "mask-type", "mark", "mark-after", "mark-before", "phonemes", "rest",
    "rest-after", "rest-before", "voice-balance", "voice-duration", "voice-pitch", "voice-pitch-range", "voice-rate", "voice-stress", "voice-volume", "marquee-direction",
    "marquee-play-count", "marquee-speed", "marquee-style"].sort();

export class HtmlExpressionComponent extends React.Component<HtmlExpressionComponentProps, void>{
    render() {
        
        return (
            <div className="form-sm code-container">
                <fieldset>
                    <legend>HTML Attributes</legend>
                    <ExpressionOrValueStrip object={this.props.htmlAttributes} filterKey={key => key != "style"} dn={this.props.dn} possibleKeys={htmlAttributeList} />
                    <fieldset>
                        <legend>CSS Properties</legend>
                        <ExpressionOrValueStrip object={this.props.htmlAttributes.style!} filterKey={key => true} dn={this.props.dn} possibleKeys={cssPropertyList} />
                    </fieldset>
                </fieldset>
            </div>
        );
    }
}



export interface ExpressionOrValueStripProps {
    possibleKeys: string[];
    dn: DesignerNode<BaseNode>;
    object: { [key: string]: ExpressionOrValue<any> }
    filterKey: (key: string) => boolean;
}

export class ExpressionOrValueStrip extends React.Component<ExpressionOrValueStripProps, void>{

    render() {

        return (
            <div>
                {this.renderList()}
                {this.renderTypeahead()}
            </div>
        );
    }

    handleOnRemove = (key: string) => {
        delete this.props.object[key];
        this.forceUpdate();
    }


    renderList() {
        return (
            <ul className="expression-list">
                {
                    Dic.getKeys(this.props.object).filter(this.props.filterKey).map(key => <li key={key}>
                        <a className="sf-line-button sf-remove"
                            onClick={() => this.handleOnRemove(key)}
                            title={EntityControlMessage.Remove.niceToString()}>
                            <span className="glyphicon glyphicon-remove"></span>
                        </a>
                        <ExpressionOrValueComponent dn={this.props.dn} refreshView={() => this.forceUpdate()}
                            binding={new Binding(this.props.object, key)} type="string" defaultValue={null} avoidDelete={true} />
                    </li>)
                }
            </ul>
        );
    }

    handleGetItems = (query: string) => {
        const result = this.props.possibleKeys
            .filter(k => k.toLowerCase().contains(query.toLowerCase()) &&
                !(this.props.object as Object).hasOwnProperty(k))
            .orderBy(a => a.length)
            .filter((k, i) => i < 5);

        return Promise.resolve(result);
    }

    handleSelect = (item: string) => {
        this.props.object[item] = undefined;
        this.forceUpdate();
        return "";
    }

    renderTypeahead() {
        return (
            <div style={{ position: "relative" }}>
                <Typeahead
                    inputAttrs={{ className: "form-control sf-entity-autocomplete" }}
                    getItems={this.handleGetItems}
                    onSelect={this.handleSelect} />
            </div>
        );
    }
}