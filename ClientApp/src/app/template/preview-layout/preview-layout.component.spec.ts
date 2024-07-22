import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { PreviewLayoutComponent } from './preview-layout.component';
import { CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA } from '@angular/core';

describe('PreviewLayoutComponent', () => {
    let component: PreviewLayoutComponent;
    let fixture: ComponentFixture<PreviewLayoutComponent>;

    beforeEach(
        waitForAsync(() => {
            TestBed.configureTestingModule({
                declarations: [PreviewLayoutComponent],
                schemas: [
                    NO_ERRORS_SCHEMA,
                    CUSTOM_ELEMENTS_SCHEMA
                  ]
            }).compileComponents();

            fixture = TestBed.createComponent(PreviewLayoutComponent);
            component = fixture.componentInstance;
            fixture.detectChanges();
        }));

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});