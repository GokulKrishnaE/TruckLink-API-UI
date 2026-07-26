import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MyTripsDriverComponent } from './my-trips-driver.component';

describe('MyTripsDriverComponent', () => {
  let component: MyTripsDriverComponent;
  let fixture: ComponentFixture<MyTripsDriverComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MyTripsDriverComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MyTripsDriverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
