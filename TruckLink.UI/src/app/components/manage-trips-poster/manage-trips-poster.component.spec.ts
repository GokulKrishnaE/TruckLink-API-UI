import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageTripsPosterComponent } from './manage-trips-poster.component';

describe('ManageTripsPosterComponent', () => {
  let component: ManageTripsPosterComponent;
  let fixture: ComponentFixture<ManageTripsPosterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ManageTripsPosterComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ManageTripsPosterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
