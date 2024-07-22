import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { EntityDataService } from '../../angular-app-services/entity-data.service';
import { DEFAULT_PAGESIZE } from '../../library/utils';
import { RendererType } from 'codezen-ui-layout';

@Component({
  selector: 'app-dynamic-layout',
  templateUrl: './dynamic-layout.component.html',
  styleUrl: './dynamic-layout.component.scss'
})
export class DynamicLayoutComponent implements OnInit, OnDestroy {
  // @Input() fieldOptions: { [key: string]: Option[]; } = {};
  @Input() form!: FormGroup;
  @Input() formFields!: any[];

  public isDropDownLoading: boolean = false;
  public rendererType = RendererType.ADD_EDIT;
  public selectorPrefix = 'use-cz-';

  private destroy = new Subject();
  private readonly pageNumber: number = 1;
  private readonly pageSize: number = DEFAULT_PAGESIZE;
  private readonly sortField: string = 'name';
  private readonly sortOrder: string = 'asc';

  constructor(
    private entityDataService: EntityDataService,
  ) { }

  ngOnInit(): void {
    this.loadDropDownData();
  }

  ngOnDestroy(): void {
    this.destroy.next(true);
    this.destroy.complete();
  }

  private loadDropDownData(): void {
    this.isDropDownLoading = true;
    const dropDownFields = this.getAllDropDownFields(this.formFields);
  
    if (dropDownFields.length === 0) {
      this.isDropDownLoading = false;
      return;
    }
  
    let completedRequests = 0;
  
    dropDownFields.forEach(field => {
      this.entityDataService.getRecords(field.dataSource)
        .pipe(takeUntil(this.destroy))
        .subscribe({
          next: (data) => 
          {
            if (Array.isArray(data)) {
            field.dataSource = data;
            field.textField = 'name';
            field.valueField = 'id';
            field.isValuePrimitive = true;
          }},
          complete: () => {
            completedRequests++;
            if (completedRequests === dropDownFields.length) {
              this.isDropDownLoading = false;
            }
          },
          error: () => {
            completedRequests++;
            if (completedRequests === dropDownFields.length) {
              this.isDropDownLoading = false;
            }
          }
        });
    });
  }
  
  private getAllDropDownFields(fields: any[]): any[] {
    let dropDownFields: any[] = [];
  
    fields.forEach(field => {
      if (field.dataSource) {
        dropDownFields.push(field);
      }
      if (field.fields && Array.isArray(field.fields)) {
        dropDownFields = dropDownFields.concat(this.getAllDropDownFields(field.fields));
      }
    });
  
    return dropDownFields;
  }
}