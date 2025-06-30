export interface JobModel{
    id?:number|undefined,
    loadItem: string,
    startLocation: string,
    destination: string,
    earnings: number,
    distanceKm: number,
    mapUrl: string
    jobId?:number
}