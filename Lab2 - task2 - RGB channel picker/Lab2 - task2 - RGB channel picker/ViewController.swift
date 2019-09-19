import Cocoa
import CoreGraphics

class ViewController: NSViewController {
    var initialImg: NSImage?
    
    required init?(coder aDecoder: NSCoder) {
        initialImg = nil
        super.init(coder: aDecoder)
    }
    
    override func prepare(for segue: NSStoryboardSegue, sender: Any?) {
        if segue.destinationController is HistogramViewController {
            let vc = segue.destinationController as? HistogramViewController
            vc?.histogramValues = histogramValues
        }
    }
    
    @IBOutlet var pickButton: NSButton!
    @IBOutlet var imageView: NSImageView!
    
    var histogramValues: (red: [Int],
                          green: [Int],
                          blue: [Int])?
    
    @IBAction func browseFile(sender: AnyObject) {
        let dialog = NSOpenPanel()
        
        dialog.title = "Choose an image"
        dialog.showsResizeIndicator = true
        dialog.showsHiddenFiles = false
        dialog.canChooseDirectories = true
        dialog.canCreateDirectories = true
        dialog.allowsMultipleSelection = false
        
        dialog.allowedFileTypes = ["png", "jpg", "jpeg"]
        
        if dialog.runModal() == NSApplication.ModalResponse.OK {
            let result = dialog.url // Pathname of the file
            
            if result != nil {
                let data = try? Data(contentsOf: result!)
                imageView.image = NSImage(data: data!)
                initialImg = imageView.image
                if let hist = calcHistogram(image: initialImg!){
                    histogramValues = hist
                }
            }
        } else {
            // User clicked on "Cancel"
            return
        }
    }
    
    func calcHistogram(image: NSImage) -> (red: [Int], green: [Int], blue: [Int])? {
        var histogram = (red: [Int](repeating: 0, count: 256),
                         green: [Int](repeating: 0, count: 256),
                         blue: [Int](repeating: 0, count: 256))
        
        let colorSpace = CGColorSpaceCreateDeviceRGB()
        
        let cgImage = imageView.image?.cgImage(forProposedRect: nil, context: nil, hints: nil)!
        
        let bitMap = cgImage!.bitmapInfo.rawValue
        let imageData = UnsafeMutablePointer<UInt32>
            .allocate(capacity: Int(image.size.width * image.size.height))
        
        let context = CGContext(data: imageData, width: Int(image.size.width),
                                height: Int(image.size.height), bitsPerComponent: 8,
                                bytesPerRow: Int(image.size.width * 4), space: colorSpace,
                                bitmapInfo: bitMap)!
        
        context.draw(cgImage!, in: CGRect(origin: CGPoint.zero, size: image.size))
        
        let pixels = UnsafeMutableBufferPointer<UInt32>(start: imageData,
                                                        count: Int(image.size.width * image.size.height))
        
        for i in 0...pixels.count - 1 {
            histogram.red[Int(pixels[i] & 0x000000FF)] += 1
            histogram.green[Int(pixels[i] & 0x0000FF00) >> 8] += 1
            histogram.blue[Int(pixels[i] & 0x00FF0000) >> 16] += 1
        }
        
        return histogram
    }
    
    func selectColorChannel(color: String, image: NSImage) -> NSImage? {
        let colorSpace = CGColorSpaceCreateDeviceRGB()
        var cgImage = imageView.image?.cgImage(forProposedRect: nil, context: nil, hints: nil)!
        let bitMap = cgImage?.bitmapInfo.rawValue
        
        let imageData = UnsafeMutablePointer<UInt32>
            .allocate(capacity: Int(image.size.width * image.size.height))
        
        let ctx = CGContext(data: imageData, width: Int(image.size.width),
                            height: Int(image.size.height), bitsPerComponent: 8,
                            bytesPerRow: Int(image.size.width * 4), space: colorSpace,
                            bitmapInfo: bitMap!)
        
        if var context = ctx {
            context.draw(cgImage!, in: CGRect(origin: CGPoint.zero, size: image.size))
            
            let pixels = UnsafeMutableBufferPointer<UInt32>(start: imageData,
                                                            count: Int(image.size.width * image.size.height))
            
            for i in 0...pixels.count - 1 {
                switch color {
                case "Red":
                    pixels[i] &= 0xFF0000FF
                case "Green":
                    pixels[i] &= 0xFF00FF00
                default:
                    pixels[i] &= 0xFFFF0000
                }
            }
            
            context = CGContext(data: pixels.baseAddress, width: Int(image.size.width),
                                height: Int(image.size.height), bitsPerComponent: 8,
                                bytesPerRow: Int(image.size.width * 4), space: colorSpace,
                                bitmapInfo: bitMap!)!
            
            cgImage = context.makeImage()!
            
       //     histogramValues = calcHistogram(image: NSImage(cgImage: cgImage!, size: CGSize(width: cgImage!.width, height: cgImage!.height)))
            
            return NSImage(cgImage: cgImage!, size: CGSize(width: cgImage!.width, height: cgImage!.height))
        } else {
            let alert = NSAlert()
            alert.alertStyle = .critical
            alert.messageText = "Image processing error"
            
            alert.addButton(withTitle: "OK")
            alert.runModal()
        }
        return nil
    }
    
    @IBAction func setInitialImg(_ sender: NSButton) {
        imageView.image = initialImg
    }
    
    @IBAction func colourBtnPressed(_ sender: NSButton) {
        if let img = selectColorChannel(color: sender.title, image: initialImg!){
            histogramValues = calcHistogram(image: img)
            imageView.image = img
        } else {
            print("error")
        }
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()
        imageView.image = #imageLiteral(resourceName: "fruits")
        initialImg = imageView.image
        histogramValues = calcHistogram(image: NSImage(cgImage: (imageView.image?.cgImage(forProposedRect: nil, context: nil, hints: nil)!)!,
                                                       size: CGSize(width: imageView.image!.size.width, height: imageView.image!.size.height)))
        
        // Do any additional setup after loading the view.
    }
    
    override var representedObject: Any? {
        didSet {
            // Update the view, if already loaded.
        }
    }
}
