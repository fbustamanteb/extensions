﻿import * as React from 'react'
import { classes, Dic } from '../../../Framework/Signum.React/Scripts/Globals'
import * as Services from '../../../Framework/Signum.React/Scripts/Services'
import * as Navigator from '../../../Framework/Signum.React/Scripts/Navigator'
import * as Constructor from '../../../Framework/Signum.React/Scripts/Constructor'
import * as Finder from '../../../Framework/Signum.React/Scripts/Finder'
import { FindOptions } from '../../../Framework/Signum.React/Scripts/FindOptions'
import { TypeContext, StyleContext, StyleOptions, FormGroupStyle } from '../../../Framework/Signum.React/Scripts/TypeContext'
import { PropertyRoute, PropertyRouteType, MemberInfo, getTypeInfo, getTypeInfos, TypeInfo, IsByAll, New, getTypeName } from '../../../Framework/Signum.React/Scripts/Reflection'
import { LineBase, LineBaseProps, FormGroup, FormControlStatic, runTasks } from '../../../Framework/Signum.React/Scripts/Lines/LineBase'
import { ModifiableEntity, Lite, Entity, EntityControlMessage, JavascriptMessage, toLite, is, liteKey, getToString } from '../../../Framework/Signum.React/Scripts/Signum.Entities'
import { IFile, IFilePath, FileMessage, FileTypeSymbol, FileEntity, FilePathEntity, FileEmbedded, FilePathEmbedded } from './Signum.Entities.Files'
import Typeahead from '../../../Framework/Signum.React/Scripts/Lines/Typeahead'
import { EntityBase, EntityBaseProps} from '../../../Framework/Signum.React/Scripts/Lines/EntityBase'

require("./Files.css");

export { FileTypeSymbol };

export interface FileUploaderProps {
    onFileLoaded: (file: IFile & ModifiableEntity) => void;
    typeName: string;
    fileType?: FileTypeSymbol;
    dragAndDrop?: boolean;
    dragAndDropMessage?: string;
    accept?: string;
    multiple?: boolean
    divHtmlAttributes?: React.HTMLAttributes<HTMLDivElement>
}

export interface FileUploaderState {
    isLoading?: boolean;
    isOver?: boolean;
}

export default class FileUploader extends React.Component<FileUploaderProps, FileUploaderState> {

    static defaultProps = {
        dragAndDrop: true
    };

    constructor(props: FileUploaderProps) {
        super(props); 

        this.state = { isLoading: false, isOver: false }; 
    }

    handleDragOver = (e: React.DragEvent<any>) => {
        e.stopPropagation();
        e.preventDefault();
        this.setState({ isOver: true });
    }

    handleDragLeave = (e: React.DragEvent<any>) => {
        e.stopPropagation();
        e.preventDefault();
        this.setState({ isOver: false });
    }

    handleDrop = (e: React.DragEvent<any>) => {
        e.stopPropagation();
        e.preventDefault();
        this.setState({
            isOver : false,
            isLoading : true
        });

        for (var i = 0; i < e.dataTransfer.files.length; i++) {
            this.uploadFile(e.dataTransfer.files[i]);
        }
    }

    handleFileChange = (e: React.FormEvent<any>) => {
        e.preventDefault();
        this.setState({
            isOver: false,
            isLoading: true
        });


        var input = e.target as HTMLInputElement;

        for (var i = 0; i < input.files!.length; i++) {
            this.uploadFile(input.files![i]);
        }
    }

    uploadFile(file: File) {
        const fileReader = new FileReader();
        fileReader.onerror = e => { setTimeout(() => { throw (e as any).error; }, 0); };
        fileReader.onload = e => {
            const fileEntity = New(this.props.typeName) as ModifiableEntity & IFile;
            fileEntity.fileName = file.name;
            fileEntity.binaryFile = ((e.target as any).result as string).after("base64,");

            if (this.props.fileType)
                (fileEntity as any as IFilePath).fileType = this.props.fileType;

            this.setState({ isLoading: false });

            this.props.onFileLoaded(fileEntity); 
        };
        fileReader.readAsDataURL(file);
    }

    render() {
        return (
            <div {...this.props.divHtmlAttributes}>
                <input type='file' className='form-control' accept={this.props.accept} onChange={this.handleFileChange} multiple={this.props.multiple}/>
                {this.state.isLoading ? <div className="sf-file-drop">{JavascriptMessage.loading.niceToString()}</div> :
                    (this.props.dragAndDrop && <div className={classes("sf-file-drop", this.state.isOver ? "sf-file-drop-over" : undefined)}
                        onDragEnter={this.handleDragOver}
                        onDragOver={this.handleDragOver}
                        onDragLeave={this.handleDragLeave}
                        onDrop={this.handleDrop}
                        >
                        {this.props.dragAndDropMessage || FileMessage.DragAndDropHere.niceToString()}
                    </div>)
                }
            </div>
        );
    }

}