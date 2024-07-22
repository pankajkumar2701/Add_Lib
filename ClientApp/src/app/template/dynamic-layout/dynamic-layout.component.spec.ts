import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { DynamicLayoutComponent } from './dynamic-layout.component';
import { EntityDataService } from '../../angular-app-services/entity-data.service';
import { of } from 'rxjs';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA } from '@angular/core';

describe('DynamicLayoutComponent', () => {
  let component: DynamicLayoutComponent;
  let fixture: ComponentFixture<DynamicLayoutComponent>;

  const entityDataServiceMock = {
    getRecords: jasmine.createSpy('getRecords').and.returnValue(of([])) // Ensure it returns an observable
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DynamicLayoutComponent],
      imports: [ReactiveFormsModule],
      providers: [
        { provide: EntityDataService, useValue: entityDataServiceMock }
      ],
      schemas: [
        NO_ERRORS_SCHEMA,
        CUSTOM_ELEMENTS_SCHEMA
      ],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DynamicLayoutComponent);
    component = fixture.componentInstance;
    component.form = new FormBuilder().group({});
    component.formFields = [
      { dataSource: 'someDataSource', fields: [] },
      { dataSource: 'anotherDataSource' }
    ];
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load dropdown data on init', fakeAsync(() => {
    const dropDownDataMock = [{ id: 1, name: 'Option 1' }, { id: 2, name: 'Option 2' }];
    entityDataServiceMock.getRecords.and.returnValue(of(dropDownDataMock));

    component.ngOnInit();
    tick();

    expect(component.isDropDownLoading).toBe(false);
    expect(component.formFields[0].dataSource).toEqual(dropDownDataMock);
    expect(component.formFields[1].dataSource).toEqual(dropDownDataMock);
  }));

  it('should clean up on destroy', () => {
    const nextSpy = spyOn(component['destroy'], 'next');
    const completeSpy = spyOn(component['destroy'], 'complete');

    component.ngOnDestroy();

    expect(nextSpy).toHaveBeenCalledWith(true);
    expect(completeSpy).toHaveBeenCalled();
  });
});
